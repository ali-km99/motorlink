using CarDealer.API.Features.Cars.DTOs;
using CarDealer.API.Features.Cars.Services.Interfaces;
using CarDealer.API.Shared.Authorization;
using CarDealer.API.Shared.Common;
using CarDealer.API.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarDealer.API.Features.Cars.Controllers;

[ApiController]
[Route("api/cars/{carId:int}/car-img")]
[Produces("application/json")]
[Authorize]

public class CarImagesController : ControllerBase
{
    private readonly ICarImageService _imageService;

    public CarImagesController(ICarImageService imageService)
    {
        _imageService = imageService;
    }

    // POST /api/cars/5/images
    [HttpPost]
    [HasPermission(PermissionCodes.CarsUploadImages)]
    [ProducesResponseType(typeof(ApiResponse<List<CarImageDto>>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [RequestSizeLimit(20 * 1024 * 1024)] // 20MB max
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload(int carId, [FromForm] List<IFormFile> files)
    {
        if (files is null || files.Count == 0)
            return BadRequest(ApiResponse<object>.Fail("No files were provided."));

        var uploaded = await _imageService.UploadImagesAsync(carId, files);
        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<List<CarImageDto>>.Ok(uploaded, $"{uploaded.Count} image(s) uploaded successfully."));
    }

    // DELETE /api/cars/5/images/12
    [HttpDelete("{imageId:int}")]
    [HasPermission(PermissionCodes.CarsDelete)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int carId,int imageId)
    {
        var deleted = await _imageService.DeleteImageAsync(imageId);
        if (!deleted)
            return NotFound(ApiResponse<object>.Fail($"Image with id {imageId} not found."));

        return Ok(ApiResponse<object>.Ok(null!, "Image deleted successfully."));
    }

    // PATCH /api/cars/5/images/12/set-primary
    [HttpPatch("{imageId:int}/set-primary")]
    [HasPermission(PermissionCodes.CarsUpdate)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetPrimary(int carId, int imageId)
    {
        var updated = await _imageService.SetPrimaryAsync(imageId);
        if (!updated)
            return NotFound(ApiResponse<object>.Fail($"Image with id {imageId} not found."));

        return Ok(ApiResponse<object>.Ok(null!, "Primary image updated."));
    }
}