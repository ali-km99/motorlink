using CarDealer.API.Features.Maintenance.Entities;
using CarDealer.API.Features.Maintenance.Repositories.Interfaces;
using CarDealer.API.Shared.Data;
using CarDealer.API.Shared.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CarDealer.API.Features.Maintenance.Repositories.Implementations;

public class MaintenancePaymentRepository : GenericRepository<MaintenancePayment>, IMaintenancePaymentRepository
{
    public MaintenancePaymentRepository(AppDbContext context) : base(context) { }

    public async Task<List<MaintenancePayment>> GetByMaintenanceIdAsync(int maintenanceId) =>
        await _context.MaintenancePayments
            .AsNoTracking()
            .Where(p => p.MaintenanceId == maintenanceId)
            .OrderBy(p => p.PaymentDate)
            .ThenBy(p => p.Id)
            .ToListAsync();

    public async Task<decimal> GetTotalPaidAsync(int maintenanceId) =>
        await _context.MaintenancePayments
            .Where(p => p.MaintenanceId == maintenanceId)
            .SumAsync(p => (decimal?)p.Amount) ?? 0;

    public async Task<bool> HasPaymentsAsync(int maintenanceId) =>
        await _context.MaintenancePayments.AnyAsync(p => p.MaintenanceId == maintenanceId);

    public async Task<bool> HasPaymentsForCarAsync(int carId) =>
        await _context.MaintenancePayments
            .AnyAsync(p => p.Maintenance.CarId == carId);
}
