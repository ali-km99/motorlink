using CarDealer.API.Data;
using CarDealer.API.DTOs;
using CarDealer.API.Entities;
using CarDealer.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarDealer.API.Services.Implementations;

public class ExpenseCategoryService : IExpenseCategoryService
{
    private readonly AppDbContext _context;

    public ExpenseCategoryService(AppDbContext context) => _context = context;

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

        var category = new ExpenseCategory { Name = name };
        _context.ExpenseCategories.Add(category);
        await _context.SaveChangesAsync();

        return new ExpenseCategoryDto(category.Id, category.Name);
    }
}