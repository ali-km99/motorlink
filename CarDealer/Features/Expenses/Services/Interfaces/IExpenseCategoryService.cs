using CarDealer.API.Features.Expenses.DTOs;

namespace CarDealer.API.Features.Expenses.Services.Interfaces
{
    public interface IExpenseCategoryService
    {
        Task<List<ExpenseCategoryDto>> GetAllAsync();
        Task<ExpenseCategoryDto> CreateAsync(CreateExpenseCategoryDto dto);
    }
}