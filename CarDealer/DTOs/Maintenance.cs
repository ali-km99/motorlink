namespace CarDealer.API.DTOs
{
    // ─── Maintenance DTOs ──────────────────────────────────────────────────────────

    public record MaintenanceDto(
        int Id,
        int CarId,
        int MaintenanceCenterId,
        string CenterName,
        string IssueDescription,
        decimal RepairCost,
        decimal TotalPaid,
        decimal RemainingAmount,
        string PaymentStatus,
        DateTime CreatedAt,
        List<MaintenancePaymentDto> Payments
    );

    public record CreateMaintenanceDto(
        int CarId,
        int MaintenanceCenterId,
        string IssueDescription,
        decimal RepairCost,
        decimal? InitialPaidAmount,
        string? PaymentNotes
    );

    public record UpdateMaintenanceDto(
        string IssueDescription,
        decimal RepairCost,
        int? MaintenanceCenterId
    );

    // ─── Maintenance Center DTOs ───────────────────────────────────────────────────

    public record MaintenanceCenterDto(
        int Id,
        string Name,
        string? Notes
    );

    public record CreateMaintenanceCenterDto(
        string Name,
        string? Notes
    );

    public record UpdateMaintenanceCenterDto(
        string Name,
        string? Notes
    );

    // ─── Maintenance Payment DTOs ──────────────────────────────────────────────────

    public record MaintenancePaymentDto(
        int Id,
        int MaintenanceId,
        decimal Amount,
        DateTime PaymentDate,
        string? Notes,
        DateTime CreatedAt
    );

    public record CreateMaintenancePaymentDto(
        decimal Amount,
        string? Notes,
        DateTime? PaymentDate
    );

    // ─── Debt DTOs ─────────────────────────────────────────────────────────────────

    public record MaintenanceDebtFilterDto(
        int? CenterId,
        int? CarId,
        string? Status,
        DateTime? DateFrom,
        DateTime? DateTo
    );

    public record MaintenanceDebtItemDto(
        int MaintenanceId,
        int CarId,
        string CarLabel,
        int MaintenanceCenterId,
        string CenterName,
        string IssueDescription,
        decimal RepairCost,
        decimal TotalPaid,
        decimal RemainingAmount,
        string PaymentStatus,
        DateTime CreatedAt
    );

    public record MaintenanceDebtCarDto(
        int CarId,
        string CarLabel,
        decimal Debt
    );

    public record MaintenanceCenterDebtDto(
        int CenterId,
        string CenterName,
        decimal TotalRepairCost,
        decimal TotalPaid,
        decimal TotalDebt,
        int UnpaidCount,
        int PartiallyPaidCount,
        int PaidCount,
        List<MaintenanceDebtCarDto> Cars
    );

    public record MaintenanceDebtReportDto(
        decimal TotalRepairCost,
        decimal TotalPaid,
        decimal TotalDebt,
        int UnpaidCount,
        int PartiallyPaidCount,
        int PaidCount,
        List<MaintenanceDebtItemDto> Items
    );
}
