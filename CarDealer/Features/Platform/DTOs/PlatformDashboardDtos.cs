namespace CarDealer.API.Features.Platform.DTOs;

// ─── طلب الفلترة الزمنية ────────────────────────────────────────────
// Preset: Last30Days | SpecificMonth | Last3Months | Last6Months | LastYear | Custom
public record PlatformDashboardFilterDto(
    string Preset = "Last30Days",
    int? Year = null,
    int? Month = null,
    DateTime? From = null,
    DateTime? To = null
);

public record DateRangeDto(DateTime From, DateTime To, string Label);

public record TenantsStatsDto(int TotalAll, int Active, int Inactive, int NewInPeriod);

public record UsersStatsDto(int TotalOwners, int TotalStaff, int NewOwnersInPeriod, int NewStaffInPeriod);

public record MarketplaceVisitsStatsDto(int TotalInPeriod, int TotalAllTime);

public record CarsStatsDto(int TotalActive, int TotalInactive, int NewInPeriod);

public record SubscriptionPlanBreakdownDto(string PlanCode, string PlanName, int ActiveTenantsCount);

public record SubscriptionsStatsDto(
    List<SubscriptionPlanBreakdownDto> PerPlan,
    int TotalActiveSubscriptions,
    int NewInPeriod
);

public record PlatformDashboardDto(
    DateRangeDto Range,
    TenantsStatsDto Tenants,
    UsersStatsDto Users,
    MarketplaceVisitsStatsDto MarketplaceVisits,
    CarsStatsDto Cars,
    SubscriptionsStatsDto Subscriptions
);