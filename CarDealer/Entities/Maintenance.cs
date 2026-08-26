namespace CarDealer.API.Entities
{
    // الصيانة
    public class Maintenance
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public int MaintenanceCenterId { get; set; }
        public string IssueDescription { get; set; } = string.Empty;
        public decimal RepairCost { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ─── Multi-Tenant (Phase 1: schema only) ─────────────────────────────────
        public int? TenantId { get; set; }
        public Tenant? Tenant { get; set; } = null!;

        public Car Car { get; set; } = null!;
        public MaintenanceCenter MaintenanceCenter { get; set; } = null!;
        public ICollection<MaintenancePayment> Payments { get; set; } = new List<MaintenancePayment>();
    }
}
