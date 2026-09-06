using CarDealer.API.Features.Platform.DTOs;
using CarDealer.API.Shared.Common;
using CarDealer.API.Shared.Data;
using Microsoft.EntityFrameworkCore;

namespace CarDealer.API.Features.Platform.Services;

public class PlatformDashboardService : IPlatformDashboardService
{
    private readonly AppDbContext _context;

    public PlatformDashboardService(AppDbContext context) => _context = context;

    public async Task<PlatformDashboardDto> GetDashboardAsync(PlatformDashboardFilterDto filter)
    {
        var (from, to, label) = ResolveRange(filter);

        // ─── Tenants ────────────────────────────────────────────────
        var totalTenants = await _context.Tenants.CountAsync();
        var activeTenants = await _context.Tenants.CountAsync(t => t.IsActive);
        var inactiveTenants = totalTenants - activeTenants;
        var newTenants = await _context.Tenants.CountAsync(t => t.CreatedAt >= from && t.CreatedAt <= to);

        // ─── Users ──────────────────────────────────────────────────
        // ملاحظة: هذا الـ Controller محمي بـ [Authorize(Roles = SuperAdmin)]، وبما أن SuperAdmin
        // دائماً IsPlatformAdmin=true، فإن فلتر TenantId التلقائي على AppUser يتجاوَز تلقائياً هنا
        // (الشرط: TenantId == currentTenant.TenantId || IsPlatformAdmin) — لا حاجة لـ IgnoreQueryFilters.
        var totalOwners = await _context.Users.CountAsync(u => u.Role == Roles.Owner);
        var totalStaff = await _context.Users.CountAsync(u => u.Role == Roles.Staff);
        var newOwners = await _context.Users.CountAsync(u => u.Role == Roles.Owner && u.CreatedAt >= from && u.CreatedAt <= to);
        var newStaff = await _context.Users.CountAsync(u => u.Role == Roles.Staff && u.CreatedAt >= from && u.CreatedAt <= to);

        // ─── Marketplace Visits ──────────────────────────────────────
        var totalVisitsInPeriod = await _context.MarketplaceVisits
            .CountAsync(v => v.VisitedAt >= from && v.VisitedAt <= to);
        var totalVisitsAllTime = await _context.MarketplaceVisits.CountAsync();

        // ─── Cars — عبر كل المعارض، متضمنة المحذوفة لعدّ "غير نشطة" بدقة ─
        // ⚠️ IgnoreQueryFilters إلزامي هنا: فلتر Car التلقائي يستثني IsDeleted=true أصلاً،
        // فلو ما تجاوزناه، عدد "غير النشطة" سيكون صفر دائماً بالخطأ.
        var totalActiveCars = await _context.Cars.IgnoreQueryFilters().CountAsync(c => !c.IsDeleted);
        var totalInactiveCars = await _context.Cars.IgnoreQueryFilters().CountAsync(c => c.IsDeleted);
        var newCars = await _context.Cars.IgnoreQueryFilters()
            .CountAsync(c => c.CreatedAt >= from && c.CreatedAt <= to);

        // ─── Subscriptions ────────────────────────────────────────────
        var perPlan = await _context.TenantSubscriptions
            .Where(s => s.IsActive)
            .GroupBy(s => new { s.SubscriptionPlan.Code, s.SubscriptionPlan.Name })
            .Select(g => new SubscriptionPlanBreakdownDto(g.Key.Code, g.Key.Name, g.Count()))
            .ToListAsync();

        var totalActiveSubscriptions = perPlan.Sum(p => p.ActiveTenantsCount);

        var newSubscriptions = await _context.TenantSubscriptions
            .CountAsync(s => s.StartedAt >= from && s.StartedAt <= to);

        return new PlatformDashboardDto(
            new DateRangeDto(from, to, label),
            new TenantsStatsDto(totalTenants, activeTenants, inactiveTenants, newTenants),
            new UsersStatsDto(totalOwners, totalStaff, newOwners, newStaff),
            new MarketplaceVisitsStatsDto(totalVisitsInPeriod, totalVisitsAllTime),
            new CarsStatsDto(totalActiveCars, totalInactiveCars, newCars),
            new SubscriptionsStatsDto(perPlan, totalActiveSubscriptions, newSubscriptions)
        );
    }

    private static (DateTime From, DateTime To, string Label) ResolveRange(PlatformDashboardFilterDto filter)
    {
        var today = DateTime.UtcNow.Date;

        return filter.Preset switch
        {
            "SpecificMonth" => ResolveSpecificMonth(filter),

            "Last3Months" => (today.AddMonths(-3), today, "Last 3 Months"),

            "Last6Months" => (today.AddMonths(-6), today, "Last 6 Months"),

            "LastYear" => (today.AddYears(-1), today, "Last Year"),

            "Custom" => ResolveCustom(filter),

            _ => (today.AddDays(-30), today, "Last 30 Days") // Default: Last30Days
        };
    }

    private static (DateTime, DateTime, string) ResolveSpecificMonth(PlatformDashboardFilterDto filter)
    {
        if (filter.Year is null || filter.Month is null)
            throw new InvalidOperationException("Year and Month are required for SpecificMonth preset.");

        var start = new DateTime(filter.Year.Value, filter.Month.Value, 1, 0, 0, 0, DateTimeKind.Utc);
        var end = start.AddMonths(1).AddSeconds(-1);
        return (start, end, $"{filter.Year:D4}-{filter.Month:D2}");
    }

    private static (DateTime, DateTime, string) ResolveCustom(PlatformDashboardFilterDto filter)
    {
        if (filter.From is null || filter.To is null)
            throw new InvalidOperationException("From and To are required for Custom preset.");

        return (filter.From.Value, filter.To.Value, "Custom");
    }
}