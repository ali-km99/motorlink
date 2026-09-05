using CarDealer.API.Entities;

namespace CarDealer.API.Features.Maintenance.Entities
{
    public class MaintenanceCenterPhone
    {
        public int Id { get; set; }
        public int MaintenanceCenterId { get; set; }
        public MaintenanceCenter MaintenanceCenter { get; set; } = null!;

        public string Label { get; set; } = string.Empty;        // مثال: "المدير", "الاستقبال", "واتساب"
        public string PhoneNumber { get; set; } = string.Empty;
        public int DisplayOrder { get; set; } = 0;

        // ─── Multi-Tenant (Phase 1: schema only) ─────────────────────────────────
        public int? TenantId { get; set; }
        public Tenant? Tenant { get; set; } = null!;
    }
}