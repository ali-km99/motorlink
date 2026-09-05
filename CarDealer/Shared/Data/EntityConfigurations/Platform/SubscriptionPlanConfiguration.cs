using CarDealer.API.Features.Platform.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarDealer.API.Shared.Data.EntityConfigurations.Platform;

public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> e)
    {
        e.HasKey(x => x.Id);
        e.Property(x => x.Code).HasMaxLength(50).IsRequired();
        e.Property(x => x.Name).HasMaxLength(100).IsRequired();
        e.HasIndex(x => x.Code).IsUnique();
        // عالمي — لا TenantId، لا فلتر

        e.HasData(
            new SubscriptionPlan
            {
                Id = 1,
                Code = "Basic",
                Name = "الأساسية",
                IsActive = true,
                AllowMaintenanceDebtReports = false,
                AllowPublicSharing = false,
                AllowExpensesModule = false
            },
            new SubscriptionPlan
            {
                Id = 2,
                Code = "Professional",
                Name = "الاحترافية",
                IsActive = true,
                AllowMaintenanceDebtReports = true,
                AllowPublicSharing = true,
                AllowExpensesModule = false
            },
            new SubscriptionPlan
            {
                Id = 3,
                Code = "Business",
                Name = "الأعمال",
                IsActive = true,
                AllowMaintenanceDebtReports = true,
                AllowPublicSharing = true,
                AllowExpensesModule = true
            }
        );
    }
}