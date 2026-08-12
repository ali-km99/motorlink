using CarDealer.API.Entities;

namespace CarDealer.API.Repositories.Interfaces
{
    // ─── Maintenance Repository ────────────────────────────────────────────────────

    public interface IMaintenanceRepository : IRepository<Maintenance>
    {
        Task<List<Maintenance>> GetByCarIdAsync(int carId);
        Task<decimal> GetTotalCostByCarIdAsync(int carId);
    }
}
