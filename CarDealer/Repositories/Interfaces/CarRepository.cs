using CarDealer.API.DTOs;
using CarDealer.API.DTOs.Car;
using CarDealer.API.Entities;

namespace CarDealer.API.Repositories.Interfaces
{
    // ─── Car Repository ────────────────────────────────────────────────────────────

    public interface ICarRepository : IRepository<Car>
    {
        Task<PagedResult<CarListDto>> GetPagedAsync(CarFilterDto filter);
        Task<Car?> GetWithDetailsAsync(int id);
        Task<List<string>> GetAllBrandsAsync();
    }
}
