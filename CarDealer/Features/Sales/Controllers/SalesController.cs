using CarDealer.API.Features.Sales.DTOs;
using CarDealer.API.Features.Sales.Services;
using CarDealer.API.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarDealer.API.Features.Sales.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class SalesController : ControllerBase
{
    private readonly ISaleService _saleService;

    public SalesController(ISaleService saleService)
    {
        _saleService = saleService;
    }

    // GET /api/sales
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<SaleListDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var sales = await _saleService.GetAllAsync();
        return Ok(ApiResponse<List<SaleListDto>>.Ok(sales));
    }

    // GET /api/sales/5
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<SaleListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var sale = await _saleService.GetByIdAsync(id);
        if (sale is null)
            return NotFound(ApiResponse<SaleListDto>.Fail($"Sale with id {id} not found."));

        return Ok(ApiResponse<SaleListDto>.Ok(sale));
    }

    // POST /api/sales
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SaleListDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateSaleDto dto)
    {
        try
        {
            var created = await _saleService.CreateSaleAsync(dto);
            return StatusCode(StatusCodes.Status201Created,
                ApiResponse<SaleListDto>.Ok(created, "Sale recorded successfully."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }
}
