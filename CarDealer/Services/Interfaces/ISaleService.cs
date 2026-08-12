using CarDealer.API.DTOs;

namespace CarDealer.API.Services.Interfaces
{
    public interface ISaleService
    {
        Task<List<SaleListDto>> GetAllAsync();
        Task<SaleListDto?> GetByIdAsync(int id);
        Task<SaleListDto> CreateSaleAsync(CreateSaleDto dto);
    }
}
