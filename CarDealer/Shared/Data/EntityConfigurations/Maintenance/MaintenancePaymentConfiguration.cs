using CarDealer.API.Features.Maintenance.Entities;
using CarDealer.API.Shared.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarDealer.API.Shared.Data.EntityConfigurations.Maintenance;

public class MaintenancePaymentConfiguration : IEntityTypeConfiguration<MaintenancePayment>
{
    private readonly ICurrentTenantService _currentTenant;

    public MaintenancePaymentConfiguration(ICurrentTenantService currentTenant)
    {
        _currentTenant = currentTenant;
    }

    public void Configure(EntityTypeBuilder<MaintenancePayment> e)
    {
        e.HasKey(x => x.Id);
        e.Property(x => x.Amount).HasColumnType("decimal(18,2)");
        e.Property(x => x.Notes).HasMaxLength(500);

        e.HasOne(x => x.Maintenance)
         .WithMany(x => x.Payments)
         .HasForeignKey(x => x.MaintenanceId)
         .OnDelete(DeleteBehavior.Restrict);

        e.HasQueryFilter(x => x.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin);

        e.HasIndex(x => x.MaintenanceId);
        e.HasIndex(x => x.PaymentDate);
    }
}