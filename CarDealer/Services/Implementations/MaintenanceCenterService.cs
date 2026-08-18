using CarDealer.API.Data;
using CarDealer.API.DTOs;
using CarDealer.API.Entities;
using CarDealer.API.Repositories.Interfaces;
using CarDealer.API.Services.Interfaces;

namespace CarDealer.API.Services.Implementations;

public class MaintenanceCenterService : IMaintenanceCenterService
{
    private readonly IMaintenanceCenterRepository _repo;
    private readonly AppDbContext _context;

    public MaintenanceCenterService(IMaintenanceCenterRepository repo, AppDbContext context)
    {
        _repo = repo;
        _context = context;
    }

    public async Task<List<MaintenanceCenterDto>> GetAllAsync()
    {
        var centers = await _repo.GetAllAsync();
        return centers.Select(ToDto).ToList();
    }

    public async Task<MaintenanceCenterDto?> GetByIdAsync(int id)
    {
        var center = await _repo.GetByIdWithPhonesAsync(id);
        return center is null ? null : ToDto(center);
    }

    public async Task<MaintenanceCenterDto> CreateAsync(CreateMaintenanceCenterDto dto)
    {
        var name = dto.Name.Trim();

        if (await _repo.ExistsByNameAsync(name))
            throw new InvalidOperationException($"مركز الصيانة '{name}' موجود مسبقاً.");

        ValidatePhones(dto.Phones);

        var center = new MaintenanceCenter
        {
            Name = name,
            Notes = dto.Notes
        };

        if (dto.Phones?.Any() == true)
        {
            center.Phones = dto.Phones
                .Select((p, index) => new MaintenanceCenterPhone
                {
                    Label = p.Label.Trim(),
                    PhoneNumber = p.PhoneNumber.Trim(),
                    DisplayOrder = index
                })
                .ToList();
        }

        await _repo.AddAsync(center);
        await _repo.SaveChangesAsync();

        return ToDto(center);
    }

    public async Task<MaintenanceCenterDto?> UpdateAsync(int id, UpdateMaintenanceCenterDto dto)
    {
        var center = await _repo.GetByIdWithPhonesAsync(id);
        if (center is null)
            return null;

        var name = dto.Name.Trim();

        if (await _repo.ExistsByNameAsync(name, id))
            throw new InvalidOperationException($"مركز الصيانة '{name}' موجود مسبقاً.");

        ValidatePhones(dto.Phones);

        center.Name = name;
        center.Notes = dto.Notes;

        // ─── استبدال كامل لأرقام الهواتف (Replace-All Pattern) ─────────────
        // نفس النمط المستخدم مع UpdateUserPermissionsAsync — أبسط وأضمن من مقارنة Diff
        _context.MaintenanceCenterPhones.RemoveRange(center.Phones);

        center.Phones = dto.Phones?.Any() == true
            ? dto.Phones
                .Select((p, index) => new MaintenanceCenterPhone
                {
                    MaintenanceCenterId = center.Id,
                    Label = p.Label.Trim(),
                    PhoneNumber = p.PhoneNumber.Trim(),
                    DisplayOrder = index
                })
                .ToList()
            : new List<MaintenanceCenterPhone>();

        await _context.SaveChangesAsync();

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

        // أرقام الهواتف تُحذف تلقائياً عبر Cascade Delete على مستوى قاعدة البيانات
        await _repo.DeleteAsync(center);
        await _repo.SaveChangesAsync();
        return true;
    }

    private static void ValidatePhones(List<CreateMaintenanceCenterPhoneDto>? phones)
    {
        if (phones is null) return;

        foreach (var p in phones)
        {
            if (string.IsNullOrWhiteSpace(p.Label))
                throw new InvalidOperationException("اسم رقم الهاتف مطلوب.");

            if (string.IsNullOrWhiteSpace(p.PhoneNumber))
                throw new InvalidOperationException("رقم الهاتف مطلوب.");
        }
    }

    private static MaintenanceCenterDto ToDto(MaintenanceCenter center) =>
        new(
            center.Id,
            center.Name,
            center.Notes,
            (center.Phones ?? new List<MaintenanceCenterPhone>())
                .OrderBy(p => p.DisplayOrder)
                .Select(p => new MaintenanceCenterPhoneDto(p.Id, p.Label, p.PhoneNumber))
                .ToList());
}