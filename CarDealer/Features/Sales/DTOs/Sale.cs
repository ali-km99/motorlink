namespace CarDealer.API.Features.Sales.DTOs
{
    // ─── Sale DTOs ─────────────────────────────────────────────────────────────────

    public record SaleInfoDto(
        int Id,
        string CustomerName,
        string CustomerPhone,
        decimal SoldPrice,
        DateTime SoldDate
    );

    public record CreateSaleDto(
        int CarId,
        int CustomerId,
        decimal SoldPrice,
        string? Notes
    );

    public record SaleListDto(
        int Id,
        string CarTitle,    // Brand + Model + Year
        string CustomerName,
        decimal SoldPrice,
        decimal Profit,
        DateTime SoldDate
    );
}
