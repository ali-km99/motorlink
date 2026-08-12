using CarDealer.API.DTOs;

namespace CarDealer.API.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<PagedResult<TransactionDto>> GetAllAsync(TransactionFilterDto filter);
        Task<TransactionDto?> GetByIdAsync(int id);
        Task<TransactionSummaryDto> GetSummaryAsync();
    }
}
