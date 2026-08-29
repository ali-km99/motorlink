using CarDealer.API.Common;
using CarDealer.API.Data;
using CarDealer.API.DTOs.Marketplace;
using CarDealer.API.Entities;
using CarDealer.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

public class MarketplaceAuthService : IMarketplaceAuthService
{
    private readonly AppDbContext _context;
    private readonly IJwtService _jwt;
    private readonly IConfiguration _config;

    public MarketplaceAuthService(AppDbContext context, IJwtService jwt, IConfiguration config)
    {
        _context = context; _jwt = jwt; _config = config;
    }

    public async Task<MarketplaceAuthResponseDto> RegisterAsync(MarketplaceRegisterDto dto)
    {
        var exists = await _context.MarketplaceUsers
            .AnyAsync(u => u.Email == dto.Email || u.Username == dto.Username);
        if (exists)
            throw new InvalidOperationException("Email or username already exists.");

        PasswordHasher.ValidateStrength(dto.Password);

        var user = new MarketplaceUser
        {
            Username = dto.Username.Trim(),
            Email = dto.Email.Trim().ToLower(),
            PasswordHash = PasswordHasher.Hash(dto.Password),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.MarketplaceUsers.Add(user);
        await _context.SaveChangesAsync();

        var response = BuildResponse(user);
        await _context.SaveChangesAsync();   // يحفظ RefreshToken بعد BuildResponse
        return response;
    }

    public async Task<MarketplaceAuthResponseDto> LoginAsync(MarketplaceLoginDto dto)
    {
        var user = await _context.MarketplaceUsers.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user is null || !user.IsActive || !PasswordHasher.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        user.LastLoginAt = DateTime.UtcNow;
        var response = BuildResponse(user);
        await _context.SaveChangesAsync();
        return response;
    }

    private MarketplaceAuthResponseDto BuildResponse(MarketplaceUser user)
    {
        var accessToken = _jwt.GenerateMarketplaceToken(user.Id, user.Email);
        var refreshToken = _jwt.GenerateRefreshToken();
        var expiry = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:AccessTokenMinutes"] ?? "60"));

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(int.Parse(_config["Jwt:RefreshTokenDays"] ?? "7"));

        return new MarketplaceAuthResponseDto(user.Id, user.Username, user.Email, accessToken, refreshToken, expiry);
    }
}