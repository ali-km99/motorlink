using CarDealer.API.Data;
using CarDealer.API.Entities;
using Microsoft.EntityFrameworkCore;
using CarDealer.API.Services;
using CarDealer.API.Features.Expenses.DTOs;
using CarDealer.API.Features.Expenses.Services.Interfaces;
using CarDealer.API.Features.Expenses.Entities;

namespace CarDealer.API.Features.Expenses.Services.Implementations;

public class ExpenseCategoryService : IExpenseCategoryService
{
    private readonly AppDbContext _context;
    private readonly ICurrentTenantService _currentTenant;

    public ExpenseCategoryService(AppDbContext context, ICurrentTenantService currentTenant)
    {
        _context = context;
        _currentTenant = currentTenant;
    }

    public async Task<List<ExpenseCategoryDto>> GetAllAsync() =>
        await _context.ExpenseCategories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new ExpenseCategoryDto(c.Id, c.Name))
            .ToListAsync();

    public async Task<ExpenseCategoryDto> CreateAsync(CreateExpenseCategoryDto dto)
    {
        var name = dto.Name.Trim();

        var exists = await _context.ExpenseCategories
            .AnyAsync(c => c.Name.ToLower() == name.ToLower());

        if (exists)
            throw new InvalidOperationException($"التصنيف '{name}' موجود مسبقًا.");

        var category = new ExpenseCategory { Name = name, TenantId = _currentTenant.TenantId };
        _context.ExpenseCategories.Add(category);
        await _context.SaveChangesAsync();

        return new ExpenseCategoryDto(category.Id, category.Name);
    }
}