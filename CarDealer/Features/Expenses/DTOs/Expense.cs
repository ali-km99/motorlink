namespace CarDealer.API.Features.Expenses.DTOs
{
    // ─── Expense Category DTOs ────────────────────────────────────────────────────

    public record ExpenseCategoryDto(int Id, string Name);

    public record CreateExpenseCategoryDto(string Name);

    // ─── Expense DTOs ──────────────────────────────────────────────────────────────

    public record ExpenseDto(
        int Id,
        int CategoryId,
        string CategoryName,
        decimal Amount,
        string? Description,
        DateTime Date,
        DateTime CreatedAt
    );

    public record CreateExpenseDto(
        int CategoryId,
        decimal Amount,
        string? Description,
        DateTime? Date
    );

    public record UpdateExpenseDto(
        int CategoryId,
        decimal Amount,
        string? Description,
        DateTime? Date
    );

    public record ExpenseFilterDto(
        int? CategoryId,
        DateTime? DateFrom,
        DateTime? DateTo,
        int Page = 1,
        int PageSize = 20
    );
}