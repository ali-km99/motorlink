using CarDealer.API.Shared.Entities;

namespace CarDealer.API.Features.Platform.Entities
{
    public class TenantSubscription
    {
        public int Id { get; set; }

        public int TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;

        public int SubscriptionPlanId { get; set; }
        public SubscriptionPlan SubscriptionPlan { get; set; } = null!;

        public bool IsActive { get; set; } = true;   // تفعيل/تعطيل يدوي من Platform Admin
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? EndedAt { get; set; }
    }
}