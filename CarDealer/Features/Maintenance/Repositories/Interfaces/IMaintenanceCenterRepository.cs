using CarDealer.API.Features.Maintenance.Entities;
using CarDealer.API.Shared.Repositories;

namespace CarDealer.API.Features.Maintenance.Repositories.Interfaces;

public interface IMaintenanceCenterRepository : IRepository<MaintenanceCenter>
{
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    Task<bool> HasMaintenancesAsync(int centerId);
    Task<MaintenanceCenter?> GetByIdWithPhonesAsync(int id);
}