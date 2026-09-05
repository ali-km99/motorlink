using CarDealer.API.Common;
using CarDealer.API.Data;
using CarDealer.API.Features.Maintenance.Entities;
using CarDealer.API.Features.Maintenance.Repositories.Interfaces;
using CarDealer.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CarDealer.API.Features.Maintenance.Repositories.Implementations;

public class MaintenanceRepository : Repository<MaintenanceEntity>, IMaintenanceRepository
{
    public MaintenanceRepository(AppDbContext context) : base(context) { }

    public async Task<List<MaintenanceEntity>> GetByCarIdAsync(int carId) =>
        await _context.Maintenances
            .AsNoTracking()
            .Include(m => m.MaintenanceCenter)
            .Include(m => m.Payments)
            .Where(m => m.CarId == carId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

    public async Task<decimal> GetTotalCostByCarIdAsync(int carId) =>
        await _context.Maintenances
            .Where(m => m.CarId == carId)
            .SumAsync(m => (decimal?)m.RepairCost) ?? 0;

    public async Task<MaintenanceEntity?> GetByIdWithDetailsAsync(int id) =>
        await _context.Maintenances
            .AsNoTracking()
            .Include(m => m.MaintenanceCenter)
            .Include(m => m.Payments)
            .Include(m => m.Car)
            .FirstOrDefaultAsync(m => m.Id == id);

    public async Task<MaintenanceEntity?> GetTrackedWithPaymentsAsync(int id) =>
        await _context.Maintenances
            .Include(m => m.Payments)
            .Include(m => m.MaintenanceCenter)
            .FirstOrDefaultAsync(m => m.Id == id);

    public async Task<List<MaintenanceEntity>> GetByCenterIdWithDetailsAsync(int centerId) =>
        await _context.Maintenances
            .AsNoTracking()
            .Include(m => m.MaintenanceCenter)
            .Include(m => m.Payments)
            .Include(m => m.Car)
            .Where(m => m.MaintenanceCenterId == centerId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

    public async Task<List<MaintenanceEntity>> GetForDebtReportAsync(
        int? centerId, int? carId, string? status, DateTime? dateFrom, DateTime? dateTo)
    {
        var query = _context.Maintenances
            .AsNoTracking()
            .Include(m => m.MaintenanceCenter)
            .Include(m => m.Payments)
            .Include(m => m.Car)
            .AsQueryable();

        if (centerId.HasValue)
            query = query.Where(m => m.MaintenanceCenterId == centerId.Value);

        if (carId.HasValue)
            query = query.Where(m => m.CarId == carId.Value);

        if (dateFrom.HasValue)
            query = query.Where(m => m.CreatedAt >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(m => m.CreatedAt <= dateTo.Value);

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalized = MaintenancePaymentStatuses.Normalize(status);
            query = normalized switch
            {
                MaintenancePaymentStatuses.Unpaid =>
                    query.Where(m => (m.Payments.Sum(p => (decimal?)p.Amount) ?? 0) == 0),
                MaintenancePaymentStatuses.PartiallyPaid =>
                    query.Where(m =>
                        (m.Payments.Sum(p => (decimal?)p.Amount) ?? 0) > 0
                        && (m.Payments.Sum(p => (decimal?)p.Amount) ?? 0) < m.RepairCost),
                MaintenancePaymentStatuses.Paid =>
                    query.Where(m => (m.Payments.Sum(p => (decimal?)p.Amount) ?? 0) >= m.RepairCost),
                _ => query
            };
        }

        return await query
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }
}
