namespace CarDealer.API.Features.Customers.DTOs
{
    // ─── Customer DTOs ─────────────────────────────────────────────────────────────

    public record CustomerDto(
        int Id,
        string Name,
        string Phone,
        string? Notes,
        int TotalPurchases
    );

    public record CreateCustomerDto(
        string Name,
        string Phone,
        string? Notes
    );

    public record UpdateCustomerDto(
        string Name,
        string Phone,
        string? Notes
    );
}
