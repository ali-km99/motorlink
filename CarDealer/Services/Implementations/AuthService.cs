using CarDealer.API.Data;
using CarDealer.API.DTOs.Auth;
using CarDealer.API.Entities;
using CarDealer.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using CarDealer.API.Common;
namespace CarDealer.API.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IJwtService _jwt;
    private readonly IConfiguration _config;
    private readonly int _refreshTokenDays;

    public AuthService(AppDbContext context, IJwtService jwt, IConfiguration config)
    {
        _context = context;
        _jwt     = jwt;
        _config  = config;
        _refreshTokenDays = int.Parse(config["Jwt:RefreshTokenDays"] ?? "7");
    }

    // ─── Login ────────────────────────────────────────────────────────────────
    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        // نجيب المستخدم بالـ Email — EF Core يستخدم Parameterized Queries تلقائياً (حماية SQL Injection)
        var user = await _context.Users
     .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user is null)
            throw new UnauthorizedAccessException("Invalid email or password.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Your account is disabled.");

        if (!PasswordHasher.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return await BuildAuthResponse(user);
    }

   

    // ─── Refresh Token ────────────────────────────────────────────────────────
    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u =>
                u.RefreshToken == dto.RefreshToken &&
                u.IsActive &&
                u.RefreshTokenExpiry > DateTime.UtcNow);

        if (user is null)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        return await BuildAuthResponse(user);
    }

    // ─── Change Password ──────────────────────────────────────────────────────
    public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user is null) return false;

        if (!PasswordHasher.Verify(dto.CurrentPassword, user.PasswordHash))
            throw new UnauthorizedAccessException("Current password is incorrect.");

        PasswordHasher.ValidateStrength(dto.NewPassword);

        user.PasswordHash = PasswordHasher.Hash(dto.NewPassword);
        user.RefreshToken    = null;    // نلغي كل الـ sessions الحالية
        user.RefreshTokenExpiry = null;

        await _context.SaveChangesAsync();
        return true;
    }

    // ─── Revoke Token (Logout) ────────────────────────────────────────────────
    public async Task<bool> RevokeTokenAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user is null) return false;

        user.RefreshToken       = null;
        user.RefreshTokenExpiry = null;

        await _context.SaveChangesAsync();
        return true;
    }

    // ─── Register Dealership (New Tenant + Owner User) ──────────────────────
    public async Task<DealershipRegistrationResponseDto> RegisterDealershipAsync(RegisterDealershipDto dto)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(dto.DealershipName))
            throw new ArgumentException("Dealership name is required.");

        if (string.IsNullOrWhiteSpace(dto.DealershipSlug))
            throw new ArgumentException("Dealership slug is required.");

        if (string.IsNullOrWhiteSpace(dto.OwnerUsername))
            throw new ArgumentException("Owner username is required.");

        if (string.IsNullOrWhiteSpace(dto.OwnerEmail))
            throw new ArgumentException("Owner email is required.");

        if (string.IsNullOrWhiteSpace(dto.OwnerPassword))
            throw new ArgumentException("Owner password is required.");

        PasswordHasher.ValidateStrength(dto.OwnerPassword);

        // Check for duplicate slug and email
        var slugExists = await _context.Tenants
            .AnyAsync(t => t.Slug.ToLower() == dto.DealershipSlug.ToLower());

        if (slugExists)
            throw new InvalidOperationException("A dealership with this slug already exists.");

        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == dto.OwnerEmail.ToLower());

        if (emailExists)
            throw new InvalidOperationException("Email already registered.");

        var usernameExists = await _context.Users
            .AnyAsync(u => u.Username == dto.OwnerUsername);

        if (usernameExists)
            throw new InvalidOperationException("Username already taken.");

        // Create new tenant
        var tenant = new Tenant
        {
            Name      = dto.DealershipName.Trim(),
            Slug      = dto.DealershipSlug.Trim().ToLower(),
            IsActive  = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Tenants.Add(tenant);
        await _context.SaveChangesAsync(); // Save tenant first to get TenantId

        // Create owner user
        var ownerUser = new AppUser
        {
            Username     = dto.OwnerUsername.Trim(),
            Email        = dto.OwnerEmail.Trim().ToLower(),
            PasswordHash = PasswordHasher.Hash(dto.OwnerPassword),
            Role = Roles.Owner,
            TenantId     = tenant.Id,
            IsPlatformAdmin = false,
            CreatedAt    = DateTime.UtcNow,
            IsActive     = true
        };

        _context.Users.Add(ownerUser);
        await _context.SaveChangesAsync();

        // Build response
        var accessToken  = _jwt.GenerateAccessToken(
            ownerUser.Id,
            ownerUser.Email,
            ownerUser.Role,
            ownerUser.TenantId,
            ownerUser.IsPlatformAdmin);

        var refreshToken = _jwt.GenerateRefreshToken();
        var expiry       = DateTime.UtcNow.AddMinutes(
            int.Parse(_config["Jwt:AccessTokenMinutes"] ?? "60"));

        // Store refresh token
        ownerUser.RefreshToken       = refreshToken;
        ownerUser.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_refreshTokenDays);
        await _context.SaveChangesAsync();

        return new DealershipRegistrationResponseDto(
            tenant.Id,
            tenant.Name,
            tenant.Slug,
            ownerUser.Id,
            ownerUser.Username,
            ownerUser.Email,
            accessToken,
            refreshToken,
            expiry
        );
    }

    // ─── Private Helpers ──────────────────────────────────────────────────────

    private async Task<AuthResponseDto> BuildAuthResponse(AppUser user)
    {
        var accessToken  = _jwt.GenerateAccessToken(
            user.Id,
            user.Email,
            user.Role,
            user.TenantId,
            user.IsPlatformAdmin);

        var refreshToken = _jwt.GenerateRefreshToken();
        var expiry       = DateTime.UtcNow.AddMinutes(
            int.Parse(_config["Jwt:AccessTokenMinutes"] ?? "60"));

        // حفظ الـ Refresh Token في قاعدة البيانات
        user.RefreshToken       = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(_refreshTokenDays);
        user.LastLoginAt        = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new AuthResponseDto(
            user.Id,
            user.Username,
            user.Email,
            user.Role,
            accessToken,
            refreshToken,
            expiry
        );
    }


}
