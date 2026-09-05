namespace CarDealer.API.Features.Marketplace.DTOs;

public record MarketplaceRegisterDto(string Username, string Email, string Password);
public record MarketplaceLoginDto(string Email, string Password);
public record MarketplaceAuthResponseDto(
    int Id, string Username, string Email,
    string AccessToken, string RefreshToken, DateTime AccessTokenExpiry);