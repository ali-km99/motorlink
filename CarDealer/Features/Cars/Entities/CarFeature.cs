using CarDealer.API.Entities;

namespace CarDealer.API.Features.Cars.Entities
{
    public class CarFeature
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public int FeatureId { get; set; }

        // ─── Multi-Tenant (Phase 1: schema only) ─────────────────────────────────
        public int? TenantId { get; set; }
        public Tenant? Tenant { get; set; } = null!;

        public Car Car { get; set; } = null!;
        public Feature Feature { get; set; } = null!;
    }
}
