using CarDealer.API.Data;
using CarDealer.API.DTOs;
using CarDealer.API.Entities;
using CarDealer.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarDealer.API.Repositories;

// ─── Maintenance Repository ────────────────────────────────────────────────────

public class MaintenanceRepository : Repository<Maintenance>, IMaintenanceRepository
{
    public MaintenanceRepository(AppDbContext context) : base(context) { }

    public async Task<List<Maintenance>> GetByCarIdAsync(int carId) =>
        await _context.Maintenances
            .AsNoTracking()
            .Where(m => m.CarId == carId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

    public async Task<decimal> GetTotalCostByCarIdAsync(int carId) =>
        await _context.Maintenances
            .Where(m => m.CarId == carId)
            .SumAsync(m => (decimal?)m.RepairCost) ?? 0;

    public async Task<Maintenance?> GetTrackedByIdAsync(int id) =>
    await _context.Maintenances
        .FirstOrDefaultAsync(m => m.Id == id);
}

// ─── Sale Repository ───────────────────────────────────────────────────────────

public class SaleRepository : Repository<Sale>, ISaleRepository
{
    public SaleRepository(AppDbContext context) : base(context) { }

    public async Task<List<SaleListDto>> GetAllWithDetailsAsync()
    {
        return await _context.Sales
            .Include(s => s.Car)
            .Include(s => s.Customer)
            .AsNoTracking()
            .OrderByDescending(s => s.SoldDate)
            .Select(s => new SaleListDto(
                s.Id,
                $"{s.Car.Brand} {s.Car.Model} {s.Car.Year}",
                s.Customer.Name,
                s.SoldPrice,
                s.SoldPrice - s.Car.CostPrice - s.Car.ShippingCost
                    - _context.Maintenances.Where(m => m.CarId == s.CarId).Sum(m => m.RepairCost),
                s.SoldDate
            ))
            .ToListAsync();
    }

    public async Task<Sale?> GetByCarIdAsync(int carId) =>
        await _context.Sales
            .Include(s => s.Customer)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.CarId == carId);
}

// ─── Customer Repository ───────────────────────────────────────────────────────

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(AppDbContext context) : base(context) { }

    public async Task<List<CustomerDto>> GetAllWithStatsAsync()
    {
        var customers = await _context.Customers
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new { c.Id, c.Name, c.Phone, c.Notes })
            .ToListAsync();

        var salesCounts = await _context.Sales
            .AsNoTracking()
            .GroupBy(s => s.CustomerId)
            .Select(g => new { CustomerId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.CustomerId, x => x.Count);

        return customers.Select(c => new CustomerDto(
            c.Id,
            c.Name,
            c.Phone,
            c.Notes,
            salesCounts.TryGetValue(c.Id, out var count) ? count : 0
        )).ToList();
    }

    public async Task<Customer?> GetByPhoneAsync(string phone) =>
        await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Phone == phone);
}