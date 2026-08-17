using CarDealer.API.DTOs;
using CarDealer.API.Entities;
using CarDealer.API.Repositories.Interfaces;
using CarDealer.API.Services.Interfaces;

namespace CarDealer.API.Services.Implementations;

public class MaintenanceCenterService : IMaintenanceCenterService
{
    private readonly IMaintenanceCenterRepository _repo;

    public MaintenanceCenterService(IMaintenanceCenterRepository repo) => _repo = repo;

    public async Task<List<MaintenanceCenterDto>> GetAllAsync()
    {
        var centers = await _repo.GetAllAsync();
        return centers.Select(ToDto).ToList();
    }

    public async Task<MaintenanceCenterDto?> GetByIdAsync(int id)
    {
        var center = await _repo.GetByIdAsync(id);
        return center is null ? null : ToDto(center);
    }

    public async Task<MaintenanceCenterDto> CreateAsync(CreateMaintenanceCenterDto dto)
    {
        var name = dto.Name.Trim();

        if (await _repo.ExistsByNameAsync(name))
            throw new InvalidOperationException($"مركز الصيانة '{name}' موجود مسبقاً.");

        var center = new MaintenanceCenter
        {
            Name = name,
            Notes = dto.Notes
        };

        await _repo.AddAsync(center);
        await _repo.SaveChangesAsync();

        return ToDto(center);
    }

    public async Task<MaintenanceCenterDto?> UpdateAsync(int id, UpdateMaintenanceCenterDto dto)
    {
        var center = await _repo.GetByIdAsync(id);
        if (center is null)
            return null;

        var name = dto.Name.Trim();

        if (await _repo.ExistsByNameAsync(name, id))
            throw new InvalidOperationException($"مركز الصيانة '{name}' موجود مسبقاً.");

        center.Name = name;
        center.Notes = dto.Notes;

        await _repo.UpdateAsync(center);
        await _repo.SaveChangesAsync();

        return ToDto(center);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var center = await _repo.GetByIdAsync(id);
        if (center is null)
            return false;

        if (await _repo.HasMaintenancesAsync(id))
            throw new InvalidOperationException(
                "لا يمكن حذف مركز صيانة مرتبط بعمليات صيانة");

        await _repo.DeleteAsync(center);
        await _repo.SaveChangesAsync();
        return true;
    }

    private static MaintenanceCenterDto ToDto(MaintenanceCenter center) =>
        new(center.Id, center.Name, center.Notes);
}
