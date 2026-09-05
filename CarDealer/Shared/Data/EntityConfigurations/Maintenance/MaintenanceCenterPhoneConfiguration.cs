using CarDealer.API.Features.Maintenance.Entities;
using CarDealer.API.Shared.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarDealer.API.Shared.Data.EntityConfigurations.Maintenance;

public class MaintenanceCenterPhoneConfiguration : IEntityTypeConfiguration<MaintenanceCenterPhone>
{
    private readonly ICurrentTenantService _currentTenant;

    public MaintenanceCenterPhoneConfiguration(ICurrentTenantService currentTenant)
    {
        _currentTenant = currentTenant;
    }

    public void Configure(EntityTypeBuilder<MaintenanceCenterPhone> e)
    {
        e.HasKey(x => x.Id);
        e.Property(x => x.Label).HasMaxLength(50).IsRequired();
        e.Property(x => x.PhoneNumber).HasMaxLength(50).IsRequired();

        e.HasOne(x => x.MaintenanceCenter)
         .WithMany(c => c.Phones)
         .HasForeignKey(x => x.MaintenanceCenterId)
         .OnDelete(DeleteBehavior.Cascade);

        e.HasQueryFilter(x => x.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin);

        e.HasIndex(x => x.MaintenanceCenterId);
    }
}