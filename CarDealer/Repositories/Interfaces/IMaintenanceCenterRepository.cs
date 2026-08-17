using CarDealer.API.Entities;

namespace CarDealer.API.Repositories.Interfaces;

public interface IMaintenanceCenterRepository : IRepository<MaintenanceCenter>
{
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    Task<bool> HasMaintenancesAsync(int centerId);
}
