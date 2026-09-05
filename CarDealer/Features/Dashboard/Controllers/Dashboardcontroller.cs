using CarDealer.API.Features.Dashboard.DTOs;
using CarDealer.API.Features.Dashboard.Services.Interfaces;
using CarDealer.API.Shared.Authorization;
using CarDealer.API.Shared.Common;
using CarDealer.API.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarDealer.API.Features.Dashboard.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService) => _dashboardService = dashboardService;

    // GET /api/dashboard/stats
    [HttpGet("stats")]
    [HasPermission(PermissionCodes.DashboardView)]
    [ProducesResponseType(typeof(ApiResponse<DashboardStatsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats()
    {
        var stats = await _dashboardService.GetStatsAsync();
        return Ok(ApiResponse<DashboardStatsDto>.Ok(stats));
    }
}