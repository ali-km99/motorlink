using CarDealer.API.Features.Cars.DTOs;
using CarDealer.API.Features.Cars.Services.Interfaces;
using CarDealer.API.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarDealer.API.Features.Cars.Controllers;

[ApiController]
[Route("api/car-statuses")]
[Produces("application/json")]
[Authorize]
public class CarStatusesController : ControllerBase
{
    private readonly ICarStatusService _carStatusService;

    public CarStatusesController(ICarStatusService carStatusService)
    {
        _carStatusService = carStatusService;
    }

    // GET /api/car-statuses
    // يُستخدم في الـ Frontend لتحميل dropdown عند إضافة أو تعديل سيارة
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<CarStatusDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var statuses = await _carStatusService.GetAllAsync();
        return Ok(ApiResponse<List<CarStatusDto>>.Ok(statuses));
    }
}
