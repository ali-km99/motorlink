namespace CarDealer.API.Entities
{
    /// <summary>
    /// Represents a single dealership (tenant) in the multi-tenant system.
    /// Every business-owned record (Car, Customer, Sale, ...) is scoped to a Tenant.
    /// </summary>
    public class Tenant
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // ─── Navigation Properties ──────────────────────────────────────────
        public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
        public ICollection<Car> Cars { get; set; } = new List<Car>();
        public ICollection<Customer> Customers { get; set; } = new List<Customer>();
        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
        public ICollection<Maintenance> Maintenances { get; set; } = new List<Maintenance>();
        public ICollection<MaintenanceCenter> MaintenanceCenters { get; set; } = new List<MaintenanceCenter>();
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
        public ICollection<ExpenseCategory> ExpenseCategories { get; set; } = new List<ExpenseCategory>();
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public ICollection<PublicShare> PublicShares { get; set; } = new List<PublicShare>();
    }
}
