namespace CarDealer.API.Entities
{
    public class MaintenanceCenter
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Notes { get; set; }

        // ─── Multi-Tenant (Phase 1: schema only) ─────────────────────────────────
        public int? TenantId { get; set; }
        public Tenant? Tenant { get; set; } = null!;

        public ICollection<Maintenance> Maintenances { get; set; } = new List<Maintenance>();
        public ICollection<MaintenanceCenterPhone> Phones { get; set; } = new List<MaintenanceCenterPhone>();
    }
}