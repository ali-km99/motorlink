namespace CarDealer.API.Features.Platform.DTOs;

public record SubscriptionPlanDto(
    int Id, string Code, string Name, bool IsActive,
    bool AllowMaintenanceDebtReports, bool AllowPublicSharing, bool AllowExpensesModule
);

public record TenantSubscriptionDto(
    int TenantId, string TenantName, int SubscriptionPlanId, string PlanCode,
    bool IsActive, DateTime StartedAt, DateTime? EndedAt
);

public record AssignSubscriptionDto(int SubscriptionPlanId);