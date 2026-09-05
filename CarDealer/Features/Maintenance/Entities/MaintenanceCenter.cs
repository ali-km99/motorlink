using CarDealer.API.Shared.Entities;

namespace CarDealer.API.Features.Maintenance.Entities
{
    public class MaintenanceCenter
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Notes { get; set; }

        // ─── Multi-Tenant (Phase 1: schema only) ─────────────────────────────────
        public int? TenantId { get; set; }
        public Tenant? Tenant { get; set; } = null!;

        public ICollection<MaintenanceEntity> Maintenances { get; set; } = []   ;
        public ICollection<MaintenanceCenterPhone> Phones { get; set; } = []    ;
    }
}