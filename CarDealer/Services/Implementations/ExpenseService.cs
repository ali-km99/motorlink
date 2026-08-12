using CarDealer.API.Data;
using CarDealer.API.DTOs;
using CarDealer.API.Entities;
using CarDealer.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarDealer.API.Services.Implementations;

public class ExpenseService : IExpenseService
{
    private readonly AppDbContext _context;

    public ExpenseService(AppDbContext context) => _context = context;

    public async Task<PagedResult<ExpenseDto>> GetAllAsync(ExpenseFilterDto filter)
    {
        var query = _context.Expenses
            .Include(e => e.Category)
            .AsNoTracking()
            .AsQueryable();

        if (filter.CategoryId.HasValue)
            query = query.Where(e => e.CategoryId == filter.CategoryId.Value);

        if (filter.DateFrom.HasValue)
            query = query.Where(e => e.Date >= filter.DateFrom.Value);

        if (filter.DateTo.HasValue)
            query = query.Where(e => e.Date <= filter.DateTo.Value);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(e => e.Date)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(e => new ExpenseDto(
                e.Id, e.CategoryId, e.Category.Name, e.Amount, e.Description, e.Date, e.CreatedAt))
            .ToListAsync();

        return new PagedResult<ExpenseDto>(
            items, totalCount, filter.Page, filter.PageSize,
            (int)Math.Ceiling(totalCount / (double)filter.PageSize));
    }

    public async Task<ExpenseDto?> GetByIdAsync(int id) =>
        await _context.Expenses
            .Include(e => e.Category)
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e => new ExpenseDto(
                e.Id, e.CategoryId, e.Category.Name, e.Amount, e.Description, e.Date, e.CreatedAt))
            .FirstOrDefaultAsync();

    public async Task<ExpenseDto> CreateAsync(CreateExpenseDto dto)
    {
        var category = await _context.ExpenseCategories.FindAsync(dto.CategoryId)
            ?? throw new InvalidOperationException("التصنيف غير موجود");

        if (dto.Amount <= 0)
            throw new InvalidOperationException("المبلغ يجب أن يكون أكبر من صفر");

        var expense = new Expense
        {
            CategoryId = dto.CategoryId,
            Amount = dto.Amount,
            Description = dto.Description,
            Date = dto.Date ?? DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            using var dbTransaction = await _context.Database.BeginTransactionAsync();

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync(); // نحتاج expense.Id فعليًا

            _context.Transactions.Add(new Transaction
            {
                Type = "Expense",
                Amount = expense.Amount,
                RelatedEntity = "Expense",
                RelatedId = expense.Id,
                Description = BuildDescription(category.Name, expense.Description),
                Date = expense.Date
            });

            await _context.SaveChangesAsync();
            await dbTransaction.CommitAsync();
        });

        return new ExpenseDto(expense.Id, expense.CategoryId, category.Name,
            expense.Amount, expense.Description, expense.Date, expense.CreatedAt);
    }

    public async Task<ExpenseDto?> UpdateAsync(int id, UpdateExpenseDto dto)
    {
        var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == id);
        if (expense is null) return null;

        var category = await _context.ExpenseCategories.FindAsync(dto.CategoryId)
            ?? throw new InvalidOperationException("التصنيف غير موجود");

        if (dto.Amount <= 0)
            throw new InvalidOperationException("المبلغ يجب أن يكون أكبر من صفر");

        expense.CategoryId = dto.CategoryId;
        expense.Amount = dto.Amount;
        expense.Description = dto.Description;
        expense.Date = dto.Date ?? expense.Date;

        await _context.SaveChangesAsync();

        // ─── مزامنة الـ Transaction المرتبطة (نفس نمط الصيانة) ──────────────
        var relatedTransaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.RelatedEntity == "Expense" && t.RelatedId == id);

        if (relatedTransaction is not null)
        {
            relatedTransaction.Amount = expense.Amount;
            relatedTransaction.Date = expense.Date;
            relatedTransaction.Description = BuildDescription(category.Name, expense.Description);
            await _context.SaveChangesAsync();
        }

        return new ExpenseDto(expense.Id, expense.CategoryId, category.Name,
            expense.Amount, expense.Description, expense.Date, expense.CreatedAt);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == id);
        if (expense is null) return false;

        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            using var dbTransaction = await _context.Database.BeginTransactionAsync();

            // Soft Delete للمصروف
            expense.IsDeleted = true;
            await _context.SaveChangesAsync();

            // Soft Delete للـ Transaction المرتبطة — يخرج تلقائيًا من كل الإجماليات
            // بفضل الـ Global Query Filter على Transaction
            await _context.Transactions
                .Where(t => t.RelatedEntity == "Expense" && t.RelatedId == id)
                .ExecuteUpdateAsync(t => t.SetProperty(x => x.IsDeleted, true));

            await dbTransaction.CommitAsync();
        });

        return true;
    }

    private static string BuildDescription(string categoryName, string? description) =>
        string.IsNullOrWhiteSpace(description) ? categoryName : $"{categoryName} - {description}";
}