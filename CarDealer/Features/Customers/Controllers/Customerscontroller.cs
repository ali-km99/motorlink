using CarDealer.API.Features.Customers.DTOs;
using CarDealer.API.Features.Customers.Services.Interfaces;
using CarDealer.API.Shared.Authorization;
using CarDealer.API.Shared.Common;
using CarDealer.API.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarDealer.API.Features.Customers.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    // GET /api/customers
    [HttpGet]
    [HasPermission(PermissionCodes.CustomersView)]
    [ProducesResponseType(typeof(ApiResponse<List<CustomerDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var customers = await _customerService.GetAllAsync();
        return Ok(ApiResponse<List<CustomerDto>>.Ok(customers));
    }

    // GET /api/customers/5
    [HttpGet("{id:int}")]
    [HasPermission(PermissionCodes.CustomersView)]
    [ProducesResponseType(typeof(ApiResponse<CustomerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        if (customer is null)
            return NotFound(ApiResponse<CustomerDto>.Fail($"Customer with id {id} not found."));

        return Ok(ApiResponse<CustomerDto>.Ok(customer));
    }

    // POST /api/customers
    [HttpPost]
    [HasPermission(PermissionCodes.CustomersCreate)]
    [ProducesResponseType(typeof(ApiResponse<CustomerDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCustomerDto dto)
    {
        var created = await _customerService.CreateAsync(dto);
        return StatusCode(StatusCodes.Status201Created,
            ApiResponse<CustomerDto>.Ok(created, "Customer created successfully."));
    }

    // PUT /api/customers/5
    [HttpPut("{id:int}")]
    [HasPermission(PermissionCodes.CustomersUpdate)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCustomerDto dto)
    {
        var updated = await _customerService.UpdateAsync(id, dto);
        if (!updated)
            return NotFound(ApiResponse<object>.Fail($"Customer with id {id} not found."));

        return Ok(ApiResponse<object>.Ok(null!, "Customer updated successfully."));
    }

    // DELETE /api/customers/5
    [HttpDelete("{id:int}")]
    [HasPermission(PermissionCodes.CustomersDelete)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _customerService.DeleteAsync(id);
        if (!deleted)
            return NotFound(ApiResponse<object>.Fail($"Customer with id {id} not found."));

        return Ok(ApiResponse<object>.Ok(null!, "Customer deleted successfully."));
    }
}