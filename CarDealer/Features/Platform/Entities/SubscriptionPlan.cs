namespace CarDealer.API.Features.Platform.Entities
{
    public class SubscriptionPlan
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;   // "Basic" / "Professional" / "Business"
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;          // هل الخطة متاحة للتفعيل حالياً

        // ─── Feature Flags (بسيطة) ─────────────────────────
        public bool AllowMaintenanceDebtReports { get; set; }
        public bool AllowPublicSharing { get; set; }
        public bool AllowExpensesModule { get; set; }

        public ICollection<TenantSubscription> TenantSubscriptions { get; set; } = new List<TenantSubscription>();
    }
}