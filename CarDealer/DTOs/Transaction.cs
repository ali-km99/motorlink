namespace CarDealer.API.DTOs
{
    // ─── Transaction DTOs ──────────────────────────────────────────────────────────

    public record TransactionDto(
        int Id,
        string Type,
        decimal Amount,
        string RelatedEntity,
        int RelatedId,
        string? Description,
        DateTime Date
    );
    // ─── Transaction Filter ────────────────────────────────────────────────────────

    public record TransactionFilterDto(
        string? Type,           // Income / Expense
        string? RelatedEntity,  // Car / Maintenance / Sale
        DateTime? DateFrom,
        DateTime? DateTo,
        int Page = 1,
        int PageSize = 20
    );

    public record TransactionSummaryDto(
        decimal TotalIncome,
        decimal TotalExpense,
        decimal NetProfit,
        int TotalTransactions
    );
}
