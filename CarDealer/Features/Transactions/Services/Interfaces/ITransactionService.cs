using CarDealer.API.Features.Transactions.DTOs;
using CarDealer.API.Shared.DTOs;

namespace CarDealer.API.Features.Transactions.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<PagedResult<TransactionDto>> GetAllAsync(TransactionFilterDto filter);
        Task<TransactionDto?> GetByIdAsync(int id);
        Task<TransactionSummaryDto> GetSummaryAsync();
    }
}
