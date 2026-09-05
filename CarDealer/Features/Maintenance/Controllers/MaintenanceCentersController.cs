using CarDealer.API.Features.Maintenance.DTOs;
using CarDealer.API.Features.Maintenance.Services.Interfaces;
using CarDealer.API.Shared.Authorization;
using CarDealer.API.Shared.Common;
using CarDealer.API.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarDealer.API.Features.Maintenance.Controllers;

[ApiController]
[Route("api/maintenance-centers")]
[Produces("application/json")]
[Authorize]
public class MaintenanceCentersController : ControllerBase
{
    private readonly IMaintenanceCenterService _centerService;
    private readonly IMaintenanceService _maintenanceService;

    public MaintenanceCentersController(
        IMaintenanceCenterService centerService,
        IMaintenanceService maintenanceService)
    {
        _centerService = centerService;
        _maintenanceService = maintenanceService;
    }

    [HttpGet]
    [HasPermission(PermissionCodes.MaintenanceView)]
    [ProducesResponseType(typeof(ApiResponse<List<MaintenanceCenterDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var items = await _centerService.GetAllAsync();
        return Ok(ApiResponse<List<MaintenanceCenterDto>>.Ok(items));
    }

    [HttpGet("{id:int}")]
    [HasPermission(PermissionCodes.MaintenanceView)]
    [ProducesResponseType(typeof(ApiResponse<MaintenanceCenterDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var center = await _centerService.GetByIdAsync(id);
        if (center is null)
            return NotFound(ApiResponse<object>.Fail("مركز الصيانة غير موجود"));

        return Ok(ApiResponse<MaintenanceCenterDto>.Ok(center));
    }

    [HttpPost]
    [HasPermission(PermissionCodes.MaintenanceCreate)]
    [ProducesResponseType(typeof(ApiResponse<MaintenanceCenterDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMaintenanceCenterDto dto)
    {
        var created = await _centerService.CreateAsync(dto);
        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<MaintenanceCenterDto>.Ok(created, "تم إضافة مركز الصيانة بنجاح"));
    }

    [HttpPut("{id:int}")]
    [HasPermission(PermissionCodes.MaintenanceUpdate)]
    [ProducesResponseType(typeof(ApiResponse<MaintenanceCenterDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMaintenanceCenterDto dto)
    {
        var updated = await _centerService.UpdateAsync(id, dto);
        if (updated is null)
            return NotFound(ApiResponse<object>.Fail("مركز الصيانة غير موجود"));

        return Ok(ApiResponse<MaintenanceCenterDto>.Ok(updated, "تم تحديث مركز الصيانة بنجاح"));
    }

    [HttpDelete("{id:int}")]
    [HasPermission(PermissionCodes.MaintenanceDelete)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _centerService.DeleteAsync(id);
        if (!deleted)
            return NotFound(ApiResponse<object>.Fail("مركز الصيانة غير موجود"));

        return Ok(ApiResponse<object>.Ok(null!, "تم حذف مركز الصيانة بنجاح"));
    }

    // GET /api/maintenance-centers/{id}/debts
    [HttpGet("{id:int}/debts")]
    [HasPermission(PermissionCodes.MaintenanceView)]
    [ProducesResponseType(typeof(ApiResponse<MaintenanceCenterDebtDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDebts(int id)
    {
        var debts = await _maintenanceService.GetCenterDebtsAsync(id);
        if (debts is null)
            return NotFound(ApiResponse<object>.Fail("مركز الصيانة غير موجود"));

        return Ok(ApiResponse<MaintenanceCenterDebtDto>.Ok(debts));
    }
}
