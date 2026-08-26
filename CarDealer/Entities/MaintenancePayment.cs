namespace CarDealer.API.Entities
{
    public class MaintenancePayment
    {
        public int Id { get; set; }
        public int MaintenanceId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ─── Multi-Tenant (Phase 1: schema only) ─────────────────────────────────
        public int? TenantId { get; set; }
        public Tenant? Tenant { get; set; } = null!;

        public Maintenance Maintenance { get; set; } = null!;
    }
}
