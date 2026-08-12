using Azure.Core;
using CarDealer.API.DTOs;
using CarDealer.API.DTOs.PublicShare;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

[ApiController]
[Route("public")]
[AllowAnonymous]
[EnableRateLimiting("PublicSharePolicy")]   // راجع فقرة الحماية بالأسفل
public class PublicController : ControllerBase
{
    private readonly IPublicShareService _service;
    public PublicController(IPublicShareService service) => _service = service;

    [HttpGet("cars/{token}")]
    public async Task<IActionResult> GetPublicCar(string token)
    {
        try
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers.UserAgent.ToString();

            var result = await _service.GetPublicCarViewAsync(token, ip, userAgent);

            if (result is null)
                return NotFound(ApiResponse<object>.Fail("الرابط غير موجود"));

            return Ok(ApiResponse<PublicCarViewDto>.Ok(result));
        }
        catch (InvalidOperationException ex) when (ex.Message == "SHARE_INACTIVE_OR_EXPIRED")
        {
            return StatusCode(403, ApiResponse<object>.Fail("هذا الرابط غير فعّال أو منتهي الصلاحية"));
        }
    }
}