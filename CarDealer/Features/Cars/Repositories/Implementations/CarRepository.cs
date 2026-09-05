using CarDealer.API.Features.Cars.DTOs;
using CarDealer.API.Features.Cars.Entities;
using CarDealer.API.Features.Cars.Repositories.Interfaces;
using CarDealer.API.Shared.Data;
using CarDealer.API.Shared.DTOs;
using CarDealer.API.Shared.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CarDealer.API.Features.Cars.Repositories.Implementations;

public class CarRepository : GenericRepository<Car>, ICarRepository
{
    public CarRepository(AppDbContext context) : base(context) { }

    public async Task<PagedResult<CarListDto>> GetPagedAsync(CarFilterDto filter)
    {
        var query = _context.Cars
            .Include(c => c.Status)
            .Include(c => c.Images)
            .AsNoTracking()
            .AsQueryable();

        // ─── Filters ──────────────────────────────────────────────────────────
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.ToLower();
            query = query.Where(c =>
                c.Brand.ToLower().Contains(term) ||
                c.Model.ToLower().Contains(term) ||
                c.ExteriorColor.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(filter.Brand))
            query = query.Where(c => c.Brand.ToLower() == filter.Brand.ToLower());

        if (filter.YearFrom.HasValue)
            query = query.Where(c => c.Year >= filter.YearFrom.Value);

        if (filter.YearTo.HasValue)
            query = query.Where(c => c.Year <= filter.YearTo.Value);

        if (filter.PriceFrom.HasValue)
            query = query.Where(c => c.SellingPrice >= filter.PriceFrom.Value);

        if (filter.PriceTo.HasValue)
            query = query.Where(c => c.SellingPrice <= filter.PriceTo.Value);

        if (filter.StatusId.HasValue)
            query = query.Where(c => c.StatusId == filter.StatusId.Value);

        if (!string.IsNullOrWhiteSpace(filter.BodyType))
            query = query.Where(c => c.BodyType == filter.BodyType);

        if (!string.IsNullOrWhiteSpace(filter.Transmission))
            query = query.Where(c => c.Transmission == filter.Transmission);

        if (!string.IsNullOrWhiteSpace(filter.Condition))
            query = query.Where(c => c.Condition == filter.Condition);

        if (!string.IsNullOrWhiteSpace(filter.FuelType))
            query = query.Where(c => c.FuelType == filter.FuelType);

        if (!string.IsNullOrWhiteSpace(filter.Specs))
            query = query.Where(c => c.Specs == filter.Specs);

        // ─── Count & Paging ───────────────────────────────────────────────────
        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(c => new CarListDto(
                c.Id,
                c.Brand,
                c.Model,
                c.Year,
                c.ExteriorColor,
                c.SellingPrice,
                c.Status.Name,
                c.Images.FirstOrDefault(i => i.IsPrimary) != null
                    ? c.Images.First(i => i.IsPrimary).ImageUrl
                    : c.Images.FirstOrDefault() != null
                        ? c.Images.First().ImageUrl
                        : null,
                c.Condition,
                c.BodyType,
                c.Mileage,
                c.MileageUnit,
                c.CreatedAt
            ))
            .ToListAsync();

        return new PagedResult<CarListDto>(
            items, totalCount, filter.Page, filter.PageSize,
            (int)Math.Ceiling(totalCount / (double)filter.PageSize));
    }

    public async Task<Car?> GetWithDetailsAsync(int id)
    {
        return await _context.Cars
            .Include(c => c.Status)
            .Include(c => c.Images)
            .Include(c => c.CarFeatures).ThenInclude(cf => cf.Feature)
            .Include(c => c.Maintenances).ThenInclude(m => m.MaintenanceCenter)
            .Include(c => c.Maintenances).ThenInclude(m => m.Payments)
            .Include(c => c.Sale).ThenInclude(s => s != null ? s.Customer : null!)
            .AsSplitQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<string>> GetAllBrandsAsync()
    {
        return await _context.Cars
            .AsNoTracking()
            .Select(c => c.Brand)
            .Distinct()
            .OrderBy(b => b)
            .ToListAsync();
    }
}