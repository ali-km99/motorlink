using CarDealer.API.Features.Cars.DTOs;
using CarDealer.API.Features.Cars.Entities;
using CarDealer.API.Shared.DTOs;
using CarDealer.API.Shared.Repositories;

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
