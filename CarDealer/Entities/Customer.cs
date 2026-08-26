namespace CarDealer.API.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public bool IsDeleted { get; set; } = false;

        // ─── Multi-Tenant (Phase 1: schema only) ─────────────────────────────────
        public int? TenantId { get; set; }
        public Tenant? Tenant { get; set; } = null!;

        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}
