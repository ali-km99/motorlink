using CarDealer.API.Features.Maintenance.Entities;
using CarDealer.API.Features.Maintenance.Repositories.Interfaces;
using CarDealer.API.Shared.Data;
using CarDealer.API.Shared.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CarDealer.API.Features.Maintenance.Repositories.Implementations;

public class MaintenanceCenterRepository : GenericRepository<MaintenanceCenter>, IMaintenanceCenterRepository
{
    public MaintenanceCenterRepository(AppDbContext context) : base(context) { }

    public override async Task<List<MaintenanceCenter>> GetAllAsync() =>
        await _context.MaintenanceCenters
            .AsNoTracking()
            .Include(c => c.Phones)
            .OrderBy(c => c.Name)
            .ToListAsync();

    public async Task<MaintenanceCenter?> GetByIdWithPhonesAsync(int id) =>
        await _context.MaintenanceCenters
            .Include(c => c.Phones)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
    {
        var query = _context.MaintenanceCenters
            .Where(c => c.Name.ToLower() == name.ToLower());

        if (excludeId.HasValue)
            query = query.Where(c => c.Id != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task<bool> HasMaintenancesAsync(int centerId) =>
        await _context.Maintenances.AnyAsync(m => m.MaintenanceCenterId == centerId);
}