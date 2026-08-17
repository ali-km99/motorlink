using CarDealer.API.Entities;

namespace CarDealer.API.Repositories.Interfaces
{
    public interface IMaintenanceRepository : IRepository<Maintenance>
    {
        Task<List<Maintenance>> GetByCarIdAsync(int carId);
        Task<decimal> GetTotalCostByCarIdAsync(int carId);
        Task<Maintenance?> GetByIdWithDetailsAsync(int id);
        Task<Maintenance?> GetTrackedWithPaymentsAsync(int id);
        Task<List<Maintenance>> GetForDebtReportAsync(
            int? centerId, int? carId, string? status, DateTime? dateFrom, DateTime? dateTo);
        Task<List<Maintenance>> GetByCenterIdWithDetailsAsync(int centerId);
    }
}
