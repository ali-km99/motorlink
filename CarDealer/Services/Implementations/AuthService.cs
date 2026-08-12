using CarDealer.API.Data;
using CarDealer.API.DTOs.Auth;
using CarDealer.API.Entities;
using CarDealer.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

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

        if (!VerifyPassword(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return await BuildAuthResponse(user);
    }

    // ─── Register ─────────────────────────────────────────────────────────────
    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        // منع التكرار
        var exists = await _context.Users
            .AnyAsync(u => u.Email == dto.Email || u.Username == dto.Username);

        if (exists)
            throw new InvalidOperationException("Email or username already exists.");

        // التحقق من قوة كلمة المرور
        ValidatePasswordStrength(dto.Password);

        var user = new AppUser
        {
            Username     = dto.Username.Trim(),
            Email        = dto.Email.Trim().ToLower(),
            PasswordHash = HashPassword(dto.Password),
            Role         = dto.Role,
            CreatedAt    = DateTime.UtcNow,
            IsActive     = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

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

        if (!VerifyPassword(dto.CurrentPassword, user.PasswordHash))
            throw new UnauthorizedAccessException("Current password is incorrect.");

        ValidatePasswordStrength(dto.NewPassword);

        user.PasswordHash    = HashPassword(dto.NewPassword);
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

    // ─── Private Helpers ──────────────────────────────────────────────────────

    private async Task<AuthResponseDto> BuildAuthResponse(AppUser user)
    {
        var accessToken  = _jwt.GenerateAccessToken(user.Id, user.Email, user.Role);
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

    // ─── Password Hashing (PBKDF2 + Salt) ────────────────────────────────────
    // أقوى من BCrypt للـ .NET native ولا يحتاج package إضافي
    private static string HashPassword(string password)
    {
        var salt = new byte[16];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);

        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            iterations: 100_000,
            HashAlgorithmName.SHA256,
            outputLength: 32);

        // نخزن: salt + hash معاً بصيغة Base64
        var combined = new byte[salt.Length + hash.Length];
        Buffer.BlockCopy(salt, 0, combined, 0,             salt.Length);
        Buffer.BlockCopy(hash, 0, combined, salt.Length, hash.Length);

        return Convert.ToBase64String(combined);
    }

    private static bool VerifyPassword(string password, string storedHash)
    {
        try
        {
            var combined = Convert.FromBase64String(storedHash);
            var salt = combined[..16];
            var stored = combined[16..];

            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations: 100_000,
                HashAlgorithmName.SHA256,
                outputLength: 32);

            // Constant-time comparison لمنع Timing Attacks
            return CryptographicOperations.FixedTimeEquals(hash, stored);
        }
        catch
        {
            return false;
        }
    }

    private static void ValidatePasswordStrength(string password)
    {
        if (password.Length < 8)
            throw new InvalidOperationException("Password must be at least 8 characters.");

        if (!password.Any(char.IsUpper))
            throw new InvalidOperationException("Password must contain at least one uppercase letter.");

        if (!password.Any(char.IsDigit))
            throw new InvalidOperationException("Password must contain at least one digit.");

    }
}
