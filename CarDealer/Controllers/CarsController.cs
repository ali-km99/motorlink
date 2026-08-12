using CarDealer.API.Authorization;
using CarDealer.API.Common;
using CarDealer.API.DTOs;
using CarDealer.API.DTOs.Car;
using CarDealer.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarDealer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class CarsController : ControllerBase
{
    private readonly ICarService _carService;

    public CarsController(ICarService carService)
    {
        _carService = carService;
    }

    // GET /api/cars?brand=Toyota&yearFrom=2020&page=1&pageSize=12
    [HttpGet]
    [HasPermission(PermissionCodes.CarsView)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<CarListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] CarFilterDto filter)
    {
        var result = await _carService.GetCarsAsync(filter);
        return Ok(ApiResponse<PagedResult<CarListDto>>.Ok(result));
    }

    // GET /api/cars/5
    [HttpGet("{id:int}")]
    [HasPermission(PermissionCodes.CarsView)]
    [ProducesResponseType(typeof(ApiResponse<CarDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var car = await _carService.GetCarByIdAsync(id);
        if (car is null)
            return NotFound(ApiResponse<CarDetailDto>.Fail($"Car with id {id} not found."));

        return Ok(ApiResponse<CarDetailDto>.Ok(car));
    }

    // POST /api/cars
    [HttpPost]
    [HasPermission(PermissionCodes.CarsCreate)]
    [ProducesResponseType(typeof(ApiResponse<CarListDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCarDto dto)
    {
        var created = await _carService.CreateCarAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            ApiResponse<CarListDto>.Ok(created, "Car created successfully."));
    }

    // PUT /api/cars/5
    [HttpPut("{id:int}")]
    [HasPermission(PermissionCodes.CarsUpdate)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCarDto dto)
    {
        var updated = await _carService.UpdateCarAsync(id, dto);
        if (!updated)
            return NotFound(ApiResponse<object>.Fail($"Car with id {id} not found."));

        return Ok(ApiResponse<object>.Ok(null!, "Car updated successfully."));
    }

    // DELETE /api/cars/5
    [HttpDelete("{id:int}")]
    [HasPermission(PermissionCodes.CarsDelete)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _carService.DeleteCarAsync(id);
        if (!deleted)
            return NotFound(ApiResponse<object>.Fail($"Car with id {id} not found."));

        return Ok(ApiResponse<object>.Ok(null!, "Car deleted successfully."));
    }

    // GET /api/cars/brands
    [HttpGet("brands")]
    [HasPermission(PermissionCodes.CarsView)]
    [ProducesResponseType(typeof(ApiResponse<List<string>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBrands()
    {
        var brands = await _carService.GetBrandsAsync();
        return Ok(ApiResponse<List<string>>.Ok(brands));
    }
}