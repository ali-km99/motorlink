namespace CarDealer.API.DTOs
{
    // ─── Maintenance DTOs ──────────────────────────────────────────────────────────

    public record MaintenanceDto(
        int Id,
        int CarId,
        string IssueDescription,
        decimal RepairCost,
        DateTime CreatedAt
    );

    public record CreateMaintenanceDto(
        int CarId,
        string IssueDescription,
        decimal RepairCost
    );
    public record UpdateMaintenanceDto(
     string IssueDescription,
     decimal RepairCost
 );
}
