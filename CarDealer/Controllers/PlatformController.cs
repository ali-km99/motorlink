
using CarDealer.API.Features.Platform.DTOs;
using CarDealer.API.Features.Platform.Entities;
using CarDealer.API.Shared.Common;
using CarDealer.API.Shared.Data;
using CarDealer.API.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarDealer.API.Controllers;

[ApiController]
[Route("api/platform")]
[Authorize(Roles = Roles.SuperAdmin)]
public class PlatformController : ControllerBase
{
    private readonly AppDbContext _context;
    public PlatformController(AppDbContext context) => _context = context;

    // GET /api/platform/subscription-plans
    [HttpGet("subscription-plans")]
    public async Task<IActionResult> GetPlans()
    {
        var plans = await _context.SubscriptionPlans
            .OrderBy(p => p.Id)
            .Select(p => new SubscriptionPlanDto(
                p.Id, p.Code, p.Name, p.IsActive,
                p.AllowMaintenanceDebtReports, p.AllowPublicSharing, p.AllowExpensesModule))
            .ToListAsync();

        return Ok(ApiResponse<List<SubscriptionPlanDto>>.Ok(plans));
    }

    // GET /api/platform/tenants — كل معرض مع خطته الحالية
    [HttpGet("tenants")]
    public async Task<IActionResult> GetTenantsWithSubscriptions()
    {
        var tenants = await _context.Tenants
            .OrderBy(t => t.Name)
            .Select(t => new
            {
                t.Id,
                t.Name,
                t.Slug,
                t.IsActive,
                Subscription = _context.TenantSubscriptions
                    .Where(s => s.TenantId == t.Id && s.IsActive)
                    .OrderByDescending(s => s.StartedAt)
                    .Select(s => new TenantSubscriptionDto(
                        s.TenantId, t.Name, s.SubscriptionPlanId, s.SubscriptionPlan.Code,
                        s.IsActive, s.StartedAt, s.EndedAt))
                    .FirstOrDefault()
            })
            .ToListAsync();

        return Ok(ApiResponse<object>.Ok(tenants));
    }

    // POST /api/platform/tenants/5/subscription — ربط/تغيير خطة المعرض
    [HttpPost("tenants/{tenantId:int}/subscription")]
    public async Task<IActionResult> AssignSubscription(int tenantId, [FromBody] AssignSubscriptionDto dto)
    {
        var tenant = await _context.Tenants.FindAsync(tenantId);
        if (tenant is null)
            return NotFound(ApiResponse<object>.Fail("المعرض غير موجود"));

        var plan = await _context.SubscriptionPlans.FindAsync(dto.SubscriptionPlanId);
        if (plan is null || !plan.IsActive)
            return BadRequest(ApiResponse<object>.Fail("الخطة غير موجودة أو غير متاحة"));

        // إنهاء أي اشتراك فعّال حالي (Replace-All Pattern — نفس نمط UserPermissions)
        var currentActive = await _context.TenantSubscriptions
            .Where(s => s.TenantId == tenantId && s.IsActive)
            .ToListAsync();

        foreach (var sub in currentActive)
        {
            sub.IsActive = false;
            sub.EndedAt = DateTime.UtcNow;
        }

        _context.TenantSubscriptions.Add(new TenantSubscription
        {
            TenantId = tenantId,
            SubscriptionPlanId = plan.Id,
            IsActive = true,
            StartedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return Ok(ApiResponse<object>.Ok(null!, $"تم ربط المعرض بخطة {plan.Name} بنجاح"));
    }

    // PATCH /api/platform/tenants/5/subscription/toggle — تعطيل الاشتراك الحالي
    [HttpPatch("tenants/{tenantId:int}/subscription/toggle")]
    public async Task<IActionResult> ToggleSubscription(int tenantId)
    {
        var subscription = await _context.TenantSubscriptions
            .Where(s => s.TenantId == tenantId && s.IsActive)
            .OrderByDescending(s => s.StartedAt)
            .FirstOrDefaultAsync();

        if (subscription is null)
            return NotFound(ApiResponse<object>.Fail("لا يوجد اشتراك فعّال لهذا المعرض"));

        subscription.IsActive = false;
        subscription.EndedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return Ok(ApiResponse<object>.Ok(null!, "تم تعطيل الاشتراك"));
    }
}