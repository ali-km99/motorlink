using CarDealer.API.Common;
using CarDealer.API.Data;
using Microsoft.EntityFrameworkCore;

namespace CarDealer.API.Services;

public class TenantFeatureService : ITenantFeatureService
{
    private readonly AppDbContext _context;
    private readonly ICurrentTenantService _currentTenant;

    public TenantFeatureService(AppDbContext context, ICurrentTenantService currentTenant)
    {
        _context = context;
        _currentTenant = currentTenant;
    }

    public async Task<bool> HasFeatureAsync(string featureCode)
    {
        if (_currentTenant.TenantId is null)
            return false; // لا معرض = لا اشتراك = لا ميزات

        var subscription = await _context.TenantSubscriptions
            .Include(s => s.SubscriptionPlan)
            .Where(s => s.TenantId == _currentTenant.TenantId && s.IsActive)
            .OrderByDescending(s => s.StartedAt)
            .FirstOrDefaultAsync();

        if (subscription is null || !subscription.SubscriptionPlan.IsActive)
            return false;

        return featureCode switch
        {
            FeatureCodes.MaintenanceDebtReports => subscription.SubscriptionPlan.AllowMaintenanceDebtReports,
            FeatureCodes.PublicSharing => subscription.SubscriptionPlan.AllowPublicSharing,
            FeatureCodes.ExpensesModule => subscription.SubscriptionPlan.AllowExpensesModule,
            _ => false
        };
    }
}