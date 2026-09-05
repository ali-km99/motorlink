using CarDealer.API.Features.Dashboard.DTOs;

namespace CarDealer.API.Features.Dashboard.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetStatsAsync();
    }
}
