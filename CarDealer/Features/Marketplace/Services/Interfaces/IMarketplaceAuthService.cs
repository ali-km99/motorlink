using CarDealer.API.Features.Marketplace.DTOs;

public interface IMarketplaceAuthService
{
    Task<MarketplaceAuthResponseDto> RegisterAsync(MarketplaceRegisterDto dto);
    Task<MarketplaceAuthResponseDto> LoginAsync(MarketplaceLoginDto dto);
}