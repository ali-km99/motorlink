using CarDealer.API.Data;
using CarDealer.API.DTOs;
using CarDealer.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarDealer.API.Services.Implementations
{
    // ─── Transaction Service ───────────────────────────────────────────────────────

    public class TransactionService : ITransactionService
    {
        private readonly AppDbContext _context;

        public TransactionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<TransactionDto>> GetAllAsync(TransactionFilterDto filter)
        {
            var query = _context.Transactions.AsNoTracking().AsQueryable();

            // ─── Filters ──────────────────────────────────────────────────────────
            if (!string.IsNullOrWhiteSpace(filter.Type))
                query = query.Where(t => t.Type == filter.Type);

            if (!string.IsNullOrWhiteSpace(filter.RelatedEntity))
                query = query.Where(t => t.RelatedEntity == filter.RelatedEntity);

            if (filter.DateFrom.HasValue)
                query = query.Where(t => t.Date >= filter.DateFrom.Value);

            if (filter.DateTo.HasValue)
                query = query.Where(t => t.Date <= filter.DateTo.Value);

            // ─── Count & Paging ───────────────────────────────────────────────────
            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(t => t.Date)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(t => new TransactionDto(
                    t.Id,
                    t.Type,
                    t.Amount,
                    t.RelatedEntity,
                    t.RelatedId,
                    t.Description,
                    t.Date
                ))
                .ToListAsync();

            return new PagedResult<TransactionDto>(
                items,
                totalCount,
                filter.Page,
                filter.PageSize,
                (int)Math.Ceiling(totalCount / (double)filter.PageSize)
            );
        }

        public async Task<TransactionDto?> GetByIdAsync(int id)
        {
            return await _context.Transactions
                .AsNoTracking()
                .Where(t => t.Id == id)
                .Select(t => new TransactionDto(
                    t.Id,
                    t.Type,
                    t.Amount,
                    t.RelatedEntity,
                    t.RelatedId,
                    t.Description,
                    t.Date
                ))
                .FirstOrDefaultAsync();
        }

        public async Task<TransactionSummaryDto> GetSummaryAsync()
        {
            var totalIncome = await _context.Transactions
                .Where(t => t.Type == "Income")
                .SumAsync(t => (decimal?)t.Amount) ?? 0;

            var totalExpense = await _context.Transactions
                .Where(t => t.Type == "Expense")
                .SumAsync(t => (decimal?)t.Amount) ?? 0;

            var totalCount = await _context.Transactions.CountAsync();

            return new TransactionSummaryDto(
                totalIncome,
                totalExpense,
                totalIncome - totalExpense,
                totalCount
            );
        }
    }
}
