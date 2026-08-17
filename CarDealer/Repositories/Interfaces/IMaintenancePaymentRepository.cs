using CarDealer.API.Entities;

namespace CarDealer.API.Repositories.Interfaces;

public interface IMaintenancePaymentRepository : IRepository<MaintenancePayment>
{
    Task<List<MaintenancePayment>> GetByMaintenanceIdAsync(int maintenanceId);
    Task<decimal> GetTotalPaidAsync(int maintenanceId);
    Task<bool> HasPaymentsAsync(int maintenanceId);
    Task<bool> HasPaymentsForCarAsync(int carId);
}
