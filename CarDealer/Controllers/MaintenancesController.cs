using CarDealer.API.DTOs;
using CarDealer.API.DTOs.Car;
using CarDealer.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarDealer.API.Controllers;

[ApiController]
[Route("api/cars/{carId:int}/maintenances")]
[Produces("application/json")]
[Authorize]
public class MaintenancesController : ControllerBase
{
    private readonly IMaintenanceService _maintenanceService;

    public MaintenancesController(IMaintenanceService maintenanceService)
    {
        _maintenanceService = maintenanceService;
    }

    // GET /api/cars/5/maintenances
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<MaintenanceDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCar(int carId)
    {
        var items = await _maintenanceService.GetByCarIdAsync(carId);
        return Ok(ApiResponse<List<MaintenanceDto>>.Ok(items));
    }

    // POST /api/cars/5/maintenances
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<MaintenanceDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(int carId, [FromBody] CreateMaintenanceDto dto)
    {
        // Ensure the carId in the route matches the DTO
        var dtoWithCarId = dto with { CarId = carId };
        var created = await _maintenanceService.CreateAsync(dtoWithCarId);

        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<MaintenanceDto>.Ok(created, "Maintenance record created successfully."));
    }



    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMaintenanceDto dto)
    {
        var result = await _maintenanceService.UpdateAsync(id, dto);
        if (result is null) return NotFound(ApiResponse<object>.Fail("الصيانة غير موجودة"));
        return Ok(ApiResponse<MaintenanceDto>.Ok(result));
    }

    // DELETE /api/cars/5/maintenances/3
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int carId, int id)
    {
        var deleted = await _maintenanceService.DeleteAsync(id);
        if (!deleted)
            return NotFound(ApiResponse<object>.Fail($"Maintenance record with id {id} not found."));

        return Ok(ApiResponse<object>.Ok(null!, "Maintenance record deleted."));
    }
}