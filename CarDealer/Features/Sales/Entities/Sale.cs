using CarDealer.API.Entities;
using CarDealer.API.Features.Cars.Entities;
using CarDealer.API.Features.Customers.Entities;

namespace CarDealer.API.Features.Sales.Entities
{
    public class Sale
    {
        public int Id { get; set; }
        public int CarId { get; set; }
        public int CustomerId { get; set; }
        public decimal SoldPrice { get; set; }
        public DateTime SoldDate { get; set; } = DateTime.UtcNow;
        public string? Notes { get; set; }

        // ─── Multi-Tenant (Phase 1: schema only) ─────────────────────────────────
        public int? TenantId { get; set; }
        public Tenant? Tenant { get; set; } = null!;

        // ─── Discounted Listing support: snapshot of which price was charged ─────
        // Values: "Original" | "Discounted" — captured at sale-creation time
        public string? PriceSource { get; set; }

        public Car Car { get; set; } = null!;
        public Customer Customer { get; set; } = null!;
    }
}
