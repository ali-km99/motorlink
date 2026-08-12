using CarDealer.API.DTOs;
using CarDealer.API.Entities;

namespace CarDealer.API.Repositories.Interfaces
{
    // ─── Sale Repository ───────────────────────────────────────────────────────────

    public interface ISaleRepository : IRepository<Sale>
    {
        Task<List<SaleListDto>> GetAllWithDetailsAsync();
        Task<Sale?> GetByCarIdAsync(int carId);
    }
}
