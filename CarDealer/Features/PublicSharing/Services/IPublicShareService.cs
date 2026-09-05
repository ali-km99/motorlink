using CarDealer.API.Features.PublicSharing.DTOs;

public interface IPublicShareService
{
    Task<GenerateShareLinkResponseDto> GenerateLinkAsync(int carId, string baseUrl, GenerateShareLinkRequestDto dto);
    Task<PublicCarViewDto?> GetPublicCarViewAsync(string token, string? ipAddress, string? userAgent);
    Task<bool> BatchToggleAsync(BatchToggleSharesDto dto);
    Task<ShareAnalyticsDto?> GetAnalyticsAsync(int carId);
}