using CarDealer.API.Features.Cars.DTOs;
using CarDealer.API.Features.Cars.Services.Interfaces;
using CarDealer.API.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarDealer.API.Features.Cars.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class FeaturesController : ControllerBase
{
    private readonly IFeatureService _featureService;

    public FeaturesController(IFeatureService featureService)
    {
        _featureService = featureService;
    }

    // GET /api/features
    // يُستخدم في الـ Frontend لتحميل الـ checkboxes عند إضافة سيارة
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<FeatureDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var features = await _featureService.GetAllAsync();
        return Ok(ApiResponse<List<FeatureDto>>.Ok(features));
    }

    // GET /api/features/5
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<FeatureDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var feature = await _featureService.GetByIdAsync(id);
        if (feature is null)
            return NotFound(ApiResponse<FeatureDto>.Fail($"Feature with id {id} not found."));

        return Ok(ApiResponse<FeatureDto>.Ok(feature));
    }

    // POST /api/features
    // { "name": "Sunroof" }
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<FeatureDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateFeatureDto dto)
    {
        try
        {
            var created = await _featureService.CreateAsync(dto);
            return StatusCode(StatusCodes.Status201Created,
                ApiResponse<FeatureDto>.Ok(created, "Feature created successfully."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    // PUT /api/features/5
    // { "name": "Panoramic Sunroof" }
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateFeatureDto dto)
    {
        try
        {
            var updated = await _featureService.UpdateAsync(id, dto);
            if (!updated)
                return NotFound(ApiResponse<object>.Fail($"Feature with id {id} not found."));

            return Ok(ApiResponse<object>.Ok(null!, "Feature updated successfully."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    // DELETE /api/features/5
    // يمنع الحذف إذا كانت الميزة مرتبطة بسيارات
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _featureService.DeleteAsync(id);
            if (!deleted)
                return NotFound(ApiResponse<object>.Fail($"Feature with id {id} not found."));

            return Ok(ApiResponse<object>.Ok(null!, "Feature deleted successfully."));
        }
        catch (InvalidOperationException ex)
        {
            // الميزة مرتبطة بسيارات — لا يمكن حذفها
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }
}
