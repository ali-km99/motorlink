using CarDealer.API.Authorization;
using CarDealer.API.Common;
using CarDealer.API.DTOs;
using CarDealer.API.Features.Expenses.DTOs;
using CarDealer.API.Features.Expenses.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarDealer.API.Features.Expenses.Controllers;

[ApiController]
[Route("api/expenses")]
[Produces("application/json")]
[Authorize]
public class ExpensesController : ControllerBase
{
    private readonly IExpenseService _expenseService;
    private readonly IExpenseCategoryService _categoryService;

    public ExpensesController(IExpenseService expenseService, IExpenseCategoryService categoryService)
    {
        _expenseService = expenseService;
        _categoryService = categoryService;
    }

    // GET /api/expenses?categoryId=1&dateFrom=2026-01-01&page=1
    [HttpGet]
    [HasPermission(PermissionCodes.ExpensesView)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ExpenseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] ExpenseFilterDto filter)
    {
        var result = await _expenseService.GetAllAsync(filter);
        return Ok(ApiResponse<PagedResult<ExpenseDto>>.Ok(result));
    }

    // GET /api/expenses/5
    [HttpGet("{id:int}")]
    [HasPermission(PermissionCodes.ExpensesView)]
    [ProducesResponseType(typeof(ApiResponse<ExpenseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var expense = await _expenseService.GetByIdAsync(id);
        if (expense is null)
            return NotFound(ApiResponse<object>.Fail($"Expense with id {id} not found."));

        return Ok(ApiResponse<ExpenseDto>.Ok(expense));
    }

    // POST /api/expenses
    [HttpPost]
    [HasPermission(PermissionCodes.ExpensesCreate)]
    [ProducesResponseType(typeof(ApiResponse<ExpenseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateExpenseDto dto)
    {
        try
        {
            var created = await _expenseService.CreateAsync(dto);
            return StatusCode(StatusCodes.Status201Created,
                ApiResponse<ExpenseDto>.Ok(created, "Expense created successfully."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    // PUT /api/expenses/5
    [HttpPut("{id:int}")]
    [HasPermission(PermissionCodes.ExpensesUpdate)]
    [ProducesResponseType(typeof(ApiResponse<ExpenseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateExpenseDto dto)
    {
        try
        {
            var updated = await _expenseService.UpdateAsync(id, dto);
            if (updated is null)
                return NotFound(ApiResponse<object>.Fail($"Expense with id {id} not found."));

            return Ok(ApiResponse<ExpenseDto>.Ok(updated, "Expense updated successfully."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }

    // DELETE /api/expenses/5  → Soft Delete
    [HttpDelete("{id:int}")]
    [HasPermission(PermissionCodes.ExpensesDelete)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _expenseService.DeleteAsync(id);
        if (!deleted)
            return NotFound(ApiResponse<object>.Fail($"Expense with id {id} not found."));

        return Ok(ApiResponse<object>.Ok(null!, "Expense deleted successfully."));
    }

    // GET /api/expenses/categories
    [HttpGet("categories")]
    [HasPermission(PermissionCodes.ExpensesView)]
    [ProducesResponseType(typeof(ApiResponse<List<ExpenseCategoryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _categoryService.GetAllAsync();
        return Ok(ApiResponse<List<ExpenseCategoryDto>>.Ok(categories));
    }

    // POST /api/expenses/categories  → إضافة تصنيف جديد بضغطة زر
    [HttpPost("categories")]
    [HasPermission(PermissionCodes.ExpensesCreate)]
    [ProducesResponseType(typeof(ApiResponse<ExpenseCategoryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCategory([FromBody] CreateExpenseCategoryDto dto)
    {
        try
        {
            var created = await _categoryService.CreateAsync(dto);
            return StatusCode(StatusCodes.Status201Created,
                ApiResponse<ExpenseCategoryDto>.Ok(created, "Category created successfully."));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<object>.Fail(ex.Message));
        }
    }
}