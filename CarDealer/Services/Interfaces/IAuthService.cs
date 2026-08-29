using CarDealer.API.DTOs.Auth;

namespace CarDealer.API.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
   
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto dto);
    Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto);
    Task<bool> RevokeTokenAsync(int userId);
    Task<DealershipRegistrationResponseDto> RegisterDealershipAsync(RegisterDealershipDto dto);
}

public interface IJwtService
{
    string GenerateAccessToken(int userId, string email, string role, int? tenantId = null, bool isPlatformAdmin = false);
    string GenerateMarketplaceToken(int userId, string email);
    string GenerateRefreshToken();
    int? ValidateAccessToken(string token);
}
