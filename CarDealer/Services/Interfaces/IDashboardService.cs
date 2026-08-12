using CarDealer.API.DTOs;

namespace CarDealer.API.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetStatsAsync();
    }
}
