using CarDealer.API.Entities;

namespace CarDealer.API.Features.Transactions.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty; // Income / Expense
        public decimal Amount { get; set; }
        public string RelatedEntity { get; set; } = string.Empty; // Car / Maintenance / Sale
        public int RelatedId { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;

        // ─── Multi-Tenant (Phase 1: schema only) ─────────────────────────────────
        public int? TenantId { get; set; }
        public Tenant? Tenant { get; set; } = null!;
    }
}
