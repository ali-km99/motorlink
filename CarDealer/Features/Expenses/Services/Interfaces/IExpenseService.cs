using CarDealer.API.DTOs;
using CarDealer.API.Features.Expenses.DTOs;

namespace CarDealer.API.Features.Expenses.Services.Interfaces
{
    public interface IExpenseService
    {
        Task<PagedResult<ExpenseDto>> GetAllAsync(ExpenseFilterDto filter);
        Task<ExpenseDto?> GetByIdAsync(int id);
        Task<ExpenseDto> CreateAsync(CreateExpenseDto dto);
        Task<ExpenseDto?> UpdateAsync(int id, UpdateExpenseDto dto);
        Task<bool> DeleteAsync(int id);
    }
}