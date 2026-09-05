using CarDealer.API.Features.Transactions.DTOs;
using CarDealer.API.Features.Transactions.Services.Interfaces;
using CarDealer.API.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarDealer.API.Features.Transactions.Controllers;
[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    // GET /api/transactions?type=Income&dateFrom=2024-01-01&page=1
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<TransactionDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] TransactionFilterDto filter)
    {
        var result = await _transactionService.GetAllAsync(filter);
        return Ok(ApiResponse<PagedResult<TransactionDto>>.Ok(result));
    }

    // GET /api/transactions/5
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<TransactionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var transaction = await _transactionService.GetByIdAsync(id);
        if (transaction is null)
            return NotFound(ApiResponse<TransactionDto>.Fail($"Transaction with id {id} not found."));

        return Ok(ApiResponse<TransactionDto>.Ok(transaction));
    }

    // GET /api/transactions/summary
    [HttpGet("summary")]
    [ProducesResponseType(typeof(ApiResponse<TransactionSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummary()
    {
        var summary = await _transactionService.GetSummaryAsync();
        return Ok(ApiResponse<TransactionSummaryDto>.Ok(summary));
    }
}
