using System.Security.Claims;
using CarDealer.API.Data;
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

        var allClaims = string.Join(" | ", principal.Claims.Select(c => $"{c.Type}={c.Value}"));
      

        if (principal.HasClaim(c => c.Type == "permission"))
            return principal;

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

     
        if (!int.TryParse(userIdClaim, out var userId))
            return principal;

        var permissionCodes = await _context.UserPermissions
            .Where(up => up.UserId == userId)
            .Select(up => up.Permission.Code)
            .ToListAsync();

        

        var identity = (ClaimsIdentity)principal.Identity;
        foreach (var code in permissionCodes)
            identity.AddClaim(new Claim("permission", code));

        return principal;
    }
}