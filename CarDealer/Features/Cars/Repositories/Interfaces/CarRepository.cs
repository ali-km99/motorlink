using CarDealer.API.DTOs;
using CarDealer.API.Features.Cars.DTOs;
using CarDealer.API.Features.Cars.Entities;
using CarDealer.API.Repositories.Interfaces;

namespace CarDealer.API.Features.Cars.Repositories.Interfaces
{
    // ─── Car Repository ────────────────────────────────────────────────────────────

    public interface ICarRepository : IRepository<Car>
    {
        Task<PagedResult<CarListDto>> GetPagedAsync(CarFilterDto filter);
        Task<Car?> GetWithDetailsAsync(int id);
        Task<List<string>> GetAllBrandsAsync();
    }
}
