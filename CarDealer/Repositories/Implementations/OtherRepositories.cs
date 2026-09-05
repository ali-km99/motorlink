using CarDealer.API.Data;
using CarDealer.API.Features.Customers.DTOs;
using CarDealer.API.Features.Customers.Entities;
using CarDealer.API.Features.Customers.Repositories.Interfaces;
using CarDealer.API.Features.Sales.DTOs;
using CarDealer.API.Features.Sales.Entities;
using CarDealer.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarDealer.API.Repositories;

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

