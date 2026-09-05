using CarDealer.API.Features.Sales.DTOs;
using CarDealer.API.Features.Sales.Entities;
using CarDealer.API.Features.Sales.Repositories.Interfaces;
using CarDealer.API.Shared.Data;
using CarDealer.API.Shared.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CarDealer.API.Features.Sales.Repositories.Implementations;

// ─── Sale Repository ───────────────────────────────────────────────────────────

public class SaleRepository : GenericRepository<Sale>, ISaleRepository
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

