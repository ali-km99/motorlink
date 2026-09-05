using CarDealer.API.Features.PublicSharing.DTOs;
using CarDealer.API.Shared.Authorization;
using CarDealer.API.Shared.Common;
using CarDealer.API.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api")]
[Authorize]
public class PublicSharesController : ControllerBase
{
    private readonly IPublicShareService _service;
    public PublicSharesController(IPublicShareService service) => _service = service;

    [HttpPost("cars/{id}/generate-share-link")]
    [HasPermission(PermissionCodes.CarsShare)]
    public async Task<IActionResult> GenerateShareLink(int id, [FromBody] GenerateShareLinkRequestDto dto)
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var result = await _service.GenerateLinkAsync(id, baseUrl, dto);
        return Ok(ApiResponse<GenerateShareLinkResponseDto>.Ok(result));
    }

    [HttpPatch("public-shares/batch-toggle")]
    [HasPermission(PermissionCodes.CarsShare)]
    public async Task<IActionResult> BatchToggle([FromBody] BatchToggleSharesDto dto)
    {
        var success = await _service.BatchToggleAsync(dto);
        return Ok(ApiResponse<object>.Ok(new { success }));
    }

    [HttpGet("cars/{id}/share-analytics")]
    [HasPermission(PermissionCodes.CarsShare)]
    public async Task<IActionResult> GetAnalytics(int id)
    {
        var result = await _service.GetAnalyticsAsync(id);
        if (result is null)
            return NotFound(ApiResponse<object>.Fail("لا يوجد روابط مشاركة لهذه السيارة"));
        return Ok(ApiResponse<ShareAnalyticsDto>.Ok(result));
    }
}