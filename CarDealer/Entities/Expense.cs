namespace CarDealer.API.Entities
{
    public class Expense
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }
        public ExpenseCategory Category { get; set; } = null!;

        public decimal Amount { get; set; }
        public string? Description { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;       // تاريخ المصروف الفعلي
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // تاريخ الإدخال بالنظام

        public bool IsDeleted { get; set; } = false;

        // ─── Multi-Tenant (Phase 1: schema only) ─────────────────────────────────
        public int? TenantId { get; set; }
        public Tenant? Tenant { get; set; } = null!;
    }
}