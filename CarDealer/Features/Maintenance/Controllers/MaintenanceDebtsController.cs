using CarDealer.API.Authorization;
using CarDealer.API.Common;
using CarDealer.API.DTOs;
using CarDealer.API.Features.Maintenance.DTOs;
using CarDealer.API.Features.Maintenance.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarDealer.API.Features.Maintenance.Controllers;

[ApiController]
[Route("api/maintenance-debts")]
[Produces("application/json")]
[Authorize]
public class MaintenanceDebtsController : ControllerBase
{
    private readonly IMaintenanceService _maintenanceService;

    public MaintenanceDebtsController(IMaintenanceService maintenanceService)
    {
        _maintenanceService = maintenanceService;
    }

    // GET /api/maintenance-debts?centerId=&carId=&status=&dateFrom=&dateTo=
    [HttpGet]
    [HasPermission(PermissionCodes.MaintenanceView)]
    [RequiresFeature(FeatureCodes.MaintenanceDebtReports)]
    [ProducesResponseType(typeof(ApiResponse<MaintenanceDebtReportDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDebts([FromQuery] MaintenanceDebtFilterDto filter)
    {
        var report = await _maintenanceService.GetDebtsAsync(filter);
        return Ok(ApiResponse<MaintenanceDebtReportDto>.Ok(report));
    }
}
