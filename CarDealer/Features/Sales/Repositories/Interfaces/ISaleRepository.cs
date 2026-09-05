using CarDealer.API.Features.Sales.DTOs;
using CarDealer.API.Features.Sales.Entities;
using CarDealer.API.Shared.Repositories;

namespace CarDealer.API.Features.Sales.Repositories.Interfaces
{
    // ─── Sale Repository ───────────────────────────────────────────────────────────

    public interface ISaleRepository : IRepository<Sale>
    {
        Task<List<SaleListDto>> GetAllWithDetailsAsync();
        Task<Sale?> GetByCarIdAsync(int carId);
    }
}
