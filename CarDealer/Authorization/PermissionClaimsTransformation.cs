using System.Security.Claims;
using CarDealer.API.Data;
using CarDealer.API.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

namespace CarDealer.API.Authorization;

public class PermissionClaimsTransformation : IClaimsTransformation
{
    private readonly AppDbContext _context;

    public PermissionClaimsTransformation(AppDbContext context) => _context = context;

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated != true)
            return principal;
        // مستخدمو Marketplace منفصلون كلياً عن جدول Users — لا حاجة (ولا يجوز) البحث عنهم هنا
        if (principal.HasClaim(c => c.Type == "user_type" && c.Value == "marketplace"))
            return principal;

        // Extract userId from 'sub' claim (JWT standard) or fall back to NameIdentifier
        var userIdClaim = principal.FindFirst("sub")?.Value 
                       ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out var userId))
            return principal;

        // Get user from database to fetch TenantId and IsPlatformAdmin
        var user = await _context.Users
            .AsNoTracking()
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            return principal;

        var identity = (ClaimsIdentity)principal.Identity;

        // Add multi-tenant claims
        if (user.IsPlatformAdmin)
        {
            identity.RemoveClaim(identity.FindFirst("is_platform_admin"));
            identity.AddClaim(new Claim("is_platform_admin", "true"));
            identity.RemoveClaim(identity.FindFirst("tenant_id"));
            identity.AddClaim(new Claim("tenant_id", string.Empty));
        }
        else
        {
            identity.RemoveClaim(identity.FindFirst("is_platform_admin"));
            identity.AddClaim(new Claim("is_platform_admin", "false"));

            // Regular tenant user: set tenant_id from TenantId property
            if (user.TenantId.HasValue)
            {
                identity.RemoveClaim(identity.FindFirst("tenant_id"));
                identity.AddClaim(new Claim("tenant_id", user.TenantId.Value.ToString()));
            }
            else
            {
                identity.RemoveClaim(identity.FindFirst("tenant_id"));
                identity.AddClaim(new Claim("tenant_id", string.Empty));
            }
        }

        // Add permission claims (existing functionality)
        if (!principal.HasClaim(c => c.Type == "permission"))
        {
            // نفلتر الصلاحيات حسب UserId
            // بالنسبة للـ Staff: نفلتر حسب TenantId للمستخدم
            // بالنسبة للـ PlatformAdmin: نأخذ كل الصلاحيات
            IQueryable<UserPermission> query = _context.UserPermissions
                .IgnoreQueryFilters()
                .Where(up => up.UserId == userId);

            if (!user.IsPlatformAdmin && user.TenantId.HasValue)
            {
                query = query.Where(up => up.TenantId == user.TenantId);
            }

            var permissionCodes = await query
                .Select(up => up.Permission.Code)
                .ToListAsync();

            foreach (var code in permissionCodes)
                identity.AddClaim(new Claim("permission", code));
        }

        return principal;
    }
}