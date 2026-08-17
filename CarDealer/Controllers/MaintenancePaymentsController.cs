using CarDealer.API.Authorization;
using CarDealer.API.Common;
using CarDealer.API.DTOs;
using CarDealer.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarDealer.API.Controllers;

[ApiController]
[Route("api/maintenances/{maintenanceId:int}")]
[Produces("application/json")]
[Authorize]
public class MaintenancePaymentsController : ControllerBase
{
    private readonly IMaintenanceService _maintenanceService;

    public MaintenancePaymentsController(IMaintenanceService maintenanceService)
    {
        _maintenanceService = maintenanceService;
    }

    [HttpGet]
    [HasPermission(PermissionCodes.MaintenanceView)]
    [ProducesResponseType(typeof(ApiResponse<MaintenanceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int maintenanceId)
    {
        var maintenance = await _maintenanceService.GetByIdAsync(maintenanceId);
        if (maintenance is null)
            return NotFound(ApiResponse<object>.Fail("الصيانة غير موجودة"));

        return Ok(ApiResponse<MaintenanceDto>.Ok(maintenance));
    }

    [HttpGet("payments")]
    [HasPermission(PermissionCodes.MaintenanceView)]
    [ProducesResponseType(typeof(ApiResponse<List<MaintenancePaymentDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPayments(int maintenanceId)
    {
        var payments = await _maintenanceService.GetPaymentsAsync(maintenanceId);
        return Ok(ApiResponse<List<MaintenancePaymentDto>>.Ok(payments));
    }

    // POST /api/maintenances/{maintenanceId}/payments
    [HttpPost("payments")]
    [HasPermission(PermissionCodes.MaintenanceCreate)]
    [ProducesResponseType(typeof(ApiResponse<MaintenancePaymentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddPayment(int maintenanceId, [FromBody] CreateMaintenancePaymentDto dto)
    {
        var payment = await _maintenanceService.AddPaymentAsync(maintenanceId, dto);
        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<MaintenancePaymentDto>.Ok(payment, "تم تسجيل الدفعة بنجاح"));
    }
}
