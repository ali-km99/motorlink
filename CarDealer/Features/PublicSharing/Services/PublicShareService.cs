using CarDealer.API.Common;
using CarDealer.API.Data;

using CarDealer.API.Entities;

using Microsoft.EntityFrameworkCore;
using CarDealer.API.Services;
using CarDealer.API.Features.PublicSharing.DTOs;
using CarDealer.API.Features.PublicSharing.Entities;
using CarDealer.API.Features.Cars.DTOs;

namespace CarDealer.API.Features.PublicSharing.Services;

public class PublicShareService : IPublicShareService
{
    private readonly IPublicShareRepository _shareRepo;
    private readonly AppDbContext _context;
    private readonly ICurrentTenantService _currentTenant;

    public PublicShareService(IPublicShareRepository shareRepo, AppDbContext context,
        ICurrentTenantService currentTenant)
    {
        _shareRepo = shareRepo;
        _context = context;
        _currentTenant = currentTenant;
    }

    public async Task<GenerateShareLinkResponseDto> GenerateLinkAsync(
        int carId, string baseUrl, GenerateShareLinkRequestDto dto)
    {
        var carExists = await _context.Cars.AnyAsync(c => c.Id == carId);
        if (!carExists)
            throw new KeyNotFoundException("السيارة غير موجودة");

        var share = new PublicShare
        {
            CarId = carId,
            Token = TokenGenerator.GenerateSecureToken(),
            IsActive = true,
            ViewsCount = 0,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = dto.ExpiresAt,
            ContactAddress = dto.ContactAddress,
            TenantId = _currentTenant.TenantId
        };

        if (dto.Contacts?.Any() == true)
        {
            share.Contacts = dto.Contacts
                .Select((c, index) => new ShareContact
                {
                    Label = c.Label,
                    Value = c.Value,
                    DisplayOrder = index,
                    TenantId = _currentTenant.TenantId
                })
                .ToList();
        }

        await _shareRepo.AddAsync(share);
        await _shareRepo.SaveChangesAsync();

        return new GenerateShareLinkResponseDto(
            $"{baseUrl.TrimEnd('/')}/public/car/{share.Token}",
            share.Token,
            share.CreatedAt);
    }

    public async Task<PublicCarViewDto?> GetPublicCarViewAsync(string token, string? ipAddress, string? userAgent)
    {
        var share = await _shareRepo.GetByTokenAsync(token);

        if (share is null)
            return null;

        if (!share.IsActive || share.ExpiresAt.HasValue && share.ExpiresAt.Value < DateTime.UtcNow)
            throw new InvalidOperationException("SHARE_INACTIVE_OR_EXPIRED");

        if (share.Car is null || share.Car.IsDeleted)
            throw new InvalidOperationException("SHARE_INACTIVE_OR_EXPIRED");

        share.ViewsCount += 1;
        _context.ShareViews.Add(new ShareView
        {
            ShareId = share.Id,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            ViewedAt = DateTime.UtcNow,
            TenantId = share.TenantId
        });
        await _context.SaveChangesAsync();

        var car = share.Car;
        var featuresGrouped = new CarFeaturesGroupedDto(
               car.CarFeatures
                  .Where(cf => cf.Feature.Category == "Technology")
                  .Select(cf => cf.Feature.Name).ToList(),
               car.CarFeatures
                  .Where(cf => cf.Feature.Category == "Interior")
                  .Select(cf => cf.Feature.Name).ToList(),
               car.CarFeatures
                  .Where(cf => cf.Feature.Category == "Exterior")
                  .Select(cf => cf.Feature.Name).ToList()
           );
        return new PublicCarViewDto(
            $"{car.Brand} {car.Model} {car.Year}",
            car.Images.Select(i => i.ImageUrl).ToList(),
            car.SellingPrice,
            car.ExteriorColor,
            car.InteriorColor,
            car.Mileage,
            car.MileageUnit,
            car.BodyType,
            car.NumberOfSeats,
            car.Transmission,
            car.Condition,
            car.FuelType,
            car.EngineSize,
        featuresGrouped,
            share.ContactAddress,
            share.Contacts
                .OrderBy(c => c.DisplayOrder)
                .Select(c => new ContactEntryDto(c.Label, c.Value))
                .ToList()
        );
    }

    public async Task<bool> BatchToggleAsync(BatchToggleSharesDto dto)
    {
        if (dto.Ids is null || dto.Ids.Count == 0)
            return false;

        var count = await _context.PublicShares
            .Where(s => dto.Ids.Contains(s.Id))
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsActive, dto.IsActive));

        return count > 0;
    }

    public async Task<ShareAnalyticsDto?> GetAnalyticsAsync(int carId)
    {
        // 🔹 جيب كل الروابط الخاصة بالسيارة
        var shares = await _context.PublicShares
            .Where(s => s.CarId == carId)
            .ToListAsync();

        if (shares.Count == 0)
            return null;

        var shareIds = shares.Select(s => s.Id).ToList();

        // 🔹 إجمالي المشاهدات لكل الروابط
        var totalViews = await _context.ShareViews
            .Where(v => shareIds.Contains(v.ShareId))
            .CountAsync();

        // 🔹 المشاهدات خلال آخر 30 يوم (كما هو عندك)
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

        var viewsRaw = await _context.ShareViews
            .Where(v => shareIds.Contains(v.ShareId) && v.ViewedAt >= thirtyDaysAgo)
            .GroupBy(v => v.ViewedAt.Date)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .OrderBy(x => x.Date)
            .ToListAsync();

        var viewsOverTime = viewsRaw
            .Select(x => new ViewsOverTimeDto(x.Date.ToString("yyyy-MM-dd"), x.Count))
            .ToList();

        // 🔥 الجديد: عدد المشاهدات لكل رابط
        var links = shares.Select(s => new ShareLinkAnalyticsDto(
            s.Id,
            s.Token,
            s.ViewsCount
        )).ToList();

        return new ShareAnalyticsDto(
            totalViews,
            viewsOverTime,
            links
        );
    }
}