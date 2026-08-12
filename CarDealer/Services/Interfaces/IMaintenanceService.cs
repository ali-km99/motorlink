using CarDealer.API.DTOs;

namespace CarDealer.API.Services.Interfaces
{
    public interface IMaintenanceService
    {
        Task<List<MaintenanceDto>> GetByCarIdAsync(int carId);
        Task<MaintenanceDto> CreateAsync(CreateMaintenanceDto dto);
        Task<bool> DeleteAsync(int id);
        Task<MaintenanceDto?> UpdateAsync(int id, UpdateMaintenanceDto dto);
    }
}
