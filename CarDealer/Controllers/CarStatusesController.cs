using CarDealer.API.DTOs;
using CarDealer.API.DTOs.Car;
using CarDealer.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarDealer.API.Controllers;

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
