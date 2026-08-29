using CarDealer.API.DTOs.Marketplace;

public interface IMarketplaceAuthService
{
    Task<MarketplaceAuthResponseDto> RegisterAsync(MarketplaceRegisterDto dto);
    Task<MarketplaceAuthResponseDto> LoginAsync(MarketplaceLoginDto dto);
}