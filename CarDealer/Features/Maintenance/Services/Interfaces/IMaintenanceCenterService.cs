using CarDealer.API.Features.Maintenance.DTOs;

namespace CarDealer.API.Features.Maintenance.Services.Interfaces;

public interface IMaintenanceCenterService
{
    Task<List<MaintenanceCenterDto>> GetAllAsync();
    Task<MaintenanceCenterDto?> GetByIdAsync(int id);
    Task<MaintenanceCenterDto> CreateAsync(CreateMaintenanceCenterDto dto);
    Task<MaintenanceCenterDto?> UpdateAsync(int id, UpdateMaintenanceCenterDto dto);
    Task<bool> DeleteAsync(int id);
}
