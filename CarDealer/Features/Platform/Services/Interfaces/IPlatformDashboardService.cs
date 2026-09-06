using CarDealer.API.Features.Platform.DTOs;

namespace CarDealer.API.Features.Platform.Services;

public interface IPlatformDashboardService
{
    Task<PlatformDashboardDto> GetDashboardAsync(PlatformDashboardFilterDto filter);
}