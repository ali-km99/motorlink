using CarDealer.API.Features.Sales.DTOs;

namespace CarDealer.API.Features.Sales.Services
{
    public interface ISaleService
    {
        Task<List<SaleListDto>> GetAllAsync();
        Task<SaleListDto?> GetByIdAsync(int id);
        Task<SaleListDto> CreateSaleAsync(CreateSaleDto dto);
    }
}
