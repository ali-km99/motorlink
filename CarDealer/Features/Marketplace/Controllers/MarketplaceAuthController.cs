using CarDealer.API.Features.Marketplace.DTOs;
using CarDealer.API.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/marketplace/auth")]
[Produces("application/json")]
public class MarketplaceAuthController : ControllerBase
{
    private readonly IMarketplaceAuthService _authService;
    public MarketplaceAuthController(IMarketplaceAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] MarketplaceRegisterDto dto)
    {
        try
        {
            var result = await _authService.RegisterAsync(dto);
            return StatusCode(201, ApiResponse<MarketplaceAuthResponseDto>.Ok(result));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] MarketplaceLoginDto dto)
    {
        try
        {
            var result = await _authService.LoginAsync(dto);
            return Ok(ApiResponse<MarketplaceAuthResponseDto>.Ok(result));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<object>.Fail(ex.Message));
        }
    }
}