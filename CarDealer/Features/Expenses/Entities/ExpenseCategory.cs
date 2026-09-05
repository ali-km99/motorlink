using CarDealer.API.Shared.Entities;

namespace CarDealer.API.Features.Expenses.Entities
{
    public class ExpenseCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // رواتب / إيجار / كهرباء وماء ...

        // ─── Multi-Tenant (Phase 1: schema only) ─────────────────────────────────
        public int? TenantId { get; set; }
        public Tenant? Tenant { get; set; } = null!;

        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}