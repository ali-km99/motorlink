
using CarDealer.API.Repositories.Interfaces;
namespace CarDealer.API.Features.Maintenance.Repositories.Interfaces
{
using CarDealer.API.Features.Maintenance.Entities;
    public interface IMaintenanceRepository : IRepository<MaintenanceEntity>
    {
        Task<List<MaintenanceEntity>> GetByCarIdAsync(int carId);
        Task<decimal> GetTotalCostByCarIdAsync(int carId);
        Task<MaintenanceEntity?> GetByIdWithDetailsAsync(int id);
        Task<MaintenanceEntity?> GetTrackedWithPaymentsAsync(int id);
        Task<List<MaintenanceEntity>> GetForDebtReportAsync(
            int? centerId, int? carId, string? status, DateTime? dateFrom, DateTime? dateTo);
        Task<List<MaintenanceEntity>> GetByCenterIdWithDetailsAsync(int centerId);
    }
}
