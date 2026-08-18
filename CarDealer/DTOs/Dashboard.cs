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
        List<RecentSaleDto> RecentSales,
        decimal TotalMaintenanceDebt,
        List<TopMaintenanceDebtDto> TopMaintenanceDebts
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

    // ─── Maintenance Debt Widgets (Dashboard) ─────────────────────────────────────

    public record TopMaintenanceDebtDto(
        int MaintenanceId,
        string CarLabel,
        string MaintenanceCenterName,
        decimal RepairCost,
        decimal RemainingAmount,
        DateTime CreatedAt
    );
}