namespace CarDealer.API.DTOs
{
    // ─── Dashboard DTOs ────────────────────────────────────────────────────────────

    public record DashboardStatsDto(
        int TotalCars,
        int AvailableCars,
        int SoldCars,
        int InMaintenanceCars,
        int inShipping,
        decimal TotalRevenue,
        decimal TotalProfit,
        decimal TotalMaintenanceCost,
        List<MonthlySalesDto> MonthlySales,
        List<RecentSaleDto> RecentSales
    );

    public record MonthlySalesDto(
        string Month,
        int Count,
        decimal Revenue
    );

    public record RecentSaleDto(
        string CarTitle,
        string CustomerName,
        decimal SoldPrice,
        decimal Profit,
        DateTime SoldDate
    );
}
