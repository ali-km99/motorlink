using CarDealer.API.Shared.Entities;

namespace CarDealer.API.Features.Cars.Entities
{
    public class CarImage
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsPrimary { get; set; } = false;

        // ─── Multi-Tenant (Phase 1: schema only) ─────────────────────────────────
        public int? TenantId { get; set; }
        public Tenant? Tenant { get; set; } = null!;

        public Car Car { get; set; } = null!;
    }
}
