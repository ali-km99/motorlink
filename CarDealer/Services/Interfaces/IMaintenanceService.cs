using CarDealer.API.DTOs;

namespace CarDealer.API.Services.Interfaces
{
    public interface IMaintenanceService
    {
        Task<List<MaintenanceDto>> GetByCarIdAsync(int carId);
        Task<MaintenanceDto?> GetByIdAsync(int id);
        Task<MaintenanceDto> CreateAsync(CreateMaintenanceDto dto);
        Task<MaintenanceDto?> UpdateAsync(int id, UpdateMaintenanceDto dto);
        Task<bool> DeleteAsync(int id);
        Task<List<MaintenancePaymentDto>> GetPaymentsAsync(int maintenanceId);
        Task<MaintenancePaymentDto> AddPaymentAsync(int maintenanceId, CreateMaintenancePaymentDto dto);
        Task<MaintenanceCenterDebtDto?> GetCenterDebtsAsync(int centerId);
        Task<MaintenanceDebtReportDto> GetDebtsAsync(MaintenanceDebtFilterDto filter);
    }
}
