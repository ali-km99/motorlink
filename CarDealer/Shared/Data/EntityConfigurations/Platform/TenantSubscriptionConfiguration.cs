using CarDealer.API.Features.Platform.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarDealer.API.Shared.Data.EntityConfigurations.Platform;

public class TenantSubscriptionConfiguration : IEntityTypeConfiguration<TenantSubscription>
{
    public void Configure(EntityTypeBuilder<TenantSubscription> e)
    {
        e.HasKey(x => x.Id);

        e.HasOne(x => x.Tenant)
         .WithMany()
         .HasForeignKey(x => x.TenantId)
         .OnDelete(DeleteBehavior.Cascade);

        e.HasOne(x => x.SubscriptionPlan)
         .WithMany(p => p.TenantSubscriptions)
         .HasForeignKey(x => x.SubscriptionPlanId)
         .OnDelete(DeleteBehavior.Restrict);

        e.HasIndex(x => x.TenantId);
        e.HasIndex(x => new { x.TenantId, x.IsActive });

        // ─── TenantSubscription — عمداً بلا HasQueryFilter تلقائي ─────────
        // (يُدار من SuperAdmin عبر كل المعارض، أو يُفحص للمعرض الحالي بفلترة صريحة عبر ITenantFeatureService)
        // Tenant الافتراضي (Id=1) يُربط تلقائياً بخطة Business — حتى لا تنكسر بيئة التطوير الحالية
        e.HasData(new TenantSubscription
        {
            Id = 1,
            TenantId = 1,
            SubscriptionPlanId = 3,
            IsActive = true,
            StartedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}