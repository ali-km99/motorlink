using CarDealer.API.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CarDealer.API.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _config;
    private readonly string _secret;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _accessTokenMinutes;

    public JwtService(IConfiguration config)
    {
        _config = config;
        _secret   = config["Jwt:Secret"]   ?? throw new InvalidOperationException("Jwt:Secret not configured.");
        _issuer   = config["Jwt:Issuer"]   ?? "CarDealerAPI";
        _audience = config["Jwt:Audience"] ?? "CarDealerClient";
        _accessTokenMinutes = int.Parse(config["Jwt:AccessTokenMinutes"] ?? "60");
    }

    // ─── Generate Access Token (JWT) ──────────────────────────────────────────
    public string GenerateAccessToken(int userId, string email, string role, int? tenantId = null, bool isPlatformAdmin = false)
    {
        var key     = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var creds   = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_accessTokenMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,   userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Role,               role),
            new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64),
            new Claim("tenant_id", tenantId?.ToString() ?? string.Empty),
            new Claim("is_platform_admin", isPlatformAdmin.ToString())
        };

        var token = new JwtSecurityToken(
            issuer:             _issuer,
            audience:           _audience,
            claims:             claims,
            notBefore:          DateTime.UtcNow,
            expires:            expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // ─── Generate Refresh Token (opaque random token) ─────────────────────────
    public string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    // ─── Validate Access Token — returns userId or null ───────────────────────
    public int? ValidateAccessToken(string token)
    {
        try
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));

            var handler    = new JwtSecurityTokenHandler();
            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey         = key,
                ValidateIssuer           = true,
                ValidIssuer              = _issuer,
                ValidateAudience         = true,
                ValidAudience            = _audience,
                ValidateLifetime         = true,
                ClockSkew                = TimeSpan.Zero  // لا نسمح بأي فارق وقت
            };

            var principal = handler.ValidateToken(token, parameters, out _);
            var sub = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);

            return int.TryParse(sub, out var userId) ? userId : null;
        }
        catch
        {
            return null;
        }
    }

    public string GenerateMarketplaceToken(int userId, string email)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_accessTokenMinutes);

        var claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim("user_type", "marketplace")   // ← يميّزه عن مستخدمي المعارض
    };

        var token = new JwtSecurityToken(
            issuer: _issuer, audience: _audience, claims: claims,
            notBefore: DateTime.UtcNow, expires: expires, signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
