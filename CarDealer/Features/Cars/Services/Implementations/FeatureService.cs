using CarDealer.API.Data;
using CarDealer.API.Features.Cars.DTOs;
using CarDealer.API.Features.Cars.Entities;
using CarDealer.API.Features.Cars.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarDealer.API.Features.Cars.Services.Implementations;

public class FeatureService : IFeatureService
{
    private readonly AppDbContext _context;


public FeatureService(AppDbContext context)
    {
        _context = context;
    }

    // GET all — مع عدد السيارات التي تستخدم كل ميزة
    public async Task<List<FeatureDto>> GetAllAsync()
    {
        var features = await _context.Features
            .AsNoTracking()
            .OrderBy(f => f.Name)
            .Select(f => new { f.Id, f.Name, f.Category })
            .ToListAsync();

        // Count منفصل — يتجنب مشكلة الـ global query filter مع الـ navigation
        var usageCounts = await _context.CarFeatures
            .AsNoTracking()
            .GroupBy(cf => cf.FeatureId)
            .Select(g => new { FeatureId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.FeatureId, x => x.Count);

        return [.. features.Select(f => new FeatureDto(
            f.Id,
            f.Name,
            f.Category,
            usageCounts.TryGetValue(f.Id, out var count) ? count : 0
        ))];
    }

    // GET by id
    public async Task<FeatureDto?> GetByIdAsync(int id)
    {
        var feature = await _context.Features
            .AsNoTracking()
            .Where(f => f.Id == id)
            .Select(f => new { f.Id, f.Name, f.Category, f.CarFeatures.Count })
            .FirstOrDefaultAsync();

        if (feature is null) return null;

        return new FeatureDto(
            feature.Id,
            feature.Name,
            feature.Category,
            feature.Count
        );
    }

    // POST — إضافة ميزة جديدة مع منع التكرار
    public async Task<FeatureDto> CreateAsync(CreateFeatureDto dto)
    {
        var exists = await _context.Features
            .AnyAsync(f => f.Name.ToLower() == dto.Name.ToLower());

        if (exists)
            throw new InvalidOperationException($"Feature '{dto.Name}' already exists.");

        // تحقق من الفئة
        if (dto.Category != "Technology" && dto.Category != "Interior" && dto.Category != "Exterior")
            throw new InvalidOperationException("Category must be: Technology, Interior, or Exterior.");

        var feature = new Feature
        {
            Name = dto.Name,
            Category = dto.Category
        };

        _context.Features.Add(feature);
        await _context.SaveChangesAsync();

        return new FeatureDto(feature.Id, feature.Name, feature.Category, 0);
    }

    // PUT — تعديل الميزة
    public async Task<bool> UpdateAsync(int id, UpdateFeatureDto dto)
    {
        var feature = await _context.Features.FindAsync(id);
        if (feature is null) return false;

        // منع التكرار مع feature آخر (غير نفسه)
        var duplicate = await _context.Features
            .AnyAsync(f => f.Name.ToLower() == dto.Name.ToLower() && f.Id != id);

        if (duplicate)
            throw new InvalidOperationException($"Feature '{dto.Name}' already exists.");

        // تحقق من الفئة
        if (dto.Category != "Technology" && dto.Category != "Interior" && dto.Category != "Exterior")
            throw new InvalidOperationException("Category must be: Technology, Interior, or Exterior.");

        feature.Name = dto.Name;
        feature.Category = dto.Category;

        await _context.SaveChangesAsync();

        return true;
    }

    // DELETE — مع منع الحذف إذا كانت مرتبطة بسيارات
    public async Task<bool> DeleteAsync(int id)
    {
        var feature = await _context.Features
            .Include(f => f.CarFeatures)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (feature is null) return false;

        if (feature.CarFeatures.Count != 0)
            throw new InvalidOperationException(
                $"Cannot delete '{feature.Name}'. It is used by {feature.CarFeatures.Count} car(s).");

        _context.Features.Remove(feature);
        await _context.SaveChangesAsync();

        return true;
    }


}
