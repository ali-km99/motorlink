using CarDealer.API.Features.Maintenance.Entities;
using CarDealer.API.Shared.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarDealer.API.Shared.Data.EntityConfigurations.Maintenance;

public class MaintenanceConfiguration : IEntityTypeConfiguration<MaintenanceEntity>
{
    private readonly ICurrentTenantService _currentTenant;

    public MaintenanceConfiguration(ICurrentTenantService currentTenant)
    {
        _currentTenant = currentTenant;
    }

    public void Configure(EntityTypeBuilder<MaintenanceEntity> e)
    {
        e.HasKey(x => x.Id);
        e.Property(x => x.IssueDescription).HasMaxLength(500).IsRequired();
        e.Property(x => x.RepairCost).HasColumnType("decimal(18,2)");

        e.HasOne(x => x.Car)
         .WithMany(x => x.Maintenances)
         .HasForeignKey(x => x.CarId)
         .OnDelete(DeleteBehavior.Restrict)
         .IsRequired(false);

        e.HasOne(x => x.MaintenanceCenter)
         .WithMany(x => x.Maintenances)
         .HasForeignKey(x => x.MaintenanceCenterId)
         .OnDelete(DeleteBehavior.Restrict);

        e.HasQueryFilter(x => x.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin);

        e.HasIndex(x => x.CarId);
        e.HasIndex(x => x.MaintenanceCenterId);
        e.HasIndex(x => new { x.MaintenanceCenterId, x.CreatedAt });
    }
}