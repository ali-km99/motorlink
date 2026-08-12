using CarDealer.API.DTOs;

namespace CarDealer.API.Services.Interfaces
{
    public interface IExpenseCategoryService
    {
        Task<List<ExpenseCategoryDto>> GetAllAsync();
        Task<ExpenseCategoryDto> CreateAsync(CreateExpenseCategoryDto dto);
    }
}