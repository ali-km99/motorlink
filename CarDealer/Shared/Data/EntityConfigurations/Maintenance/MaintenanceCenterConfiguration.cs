using CarDealer.API.Features.Maintenance.Entities;
using CarDealer.API.Shared.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarDealer.API.Shared.Data.EntityConfigurations.Maintenance;

public class MaintenanceCenterConfiguration : IEntityTypeConfiguration<MaintenanceCenter>
{
    private readonly ICurrentTenantService _currentTenant;

    public MaintenanceCenterConfiguration(ICurrentTenantService currentTenant)
    {
        _currentTenant = currentTenant;
    }

    public void Configure(EntityTypeBuilder<MaintenanceCenter> e)
    {
        e.HasKey(x => x.Id);
        e.Property(x => x.Name).HasMaxLength(200).IsRequired();
        e.Property(x => x.Notes).HasMaxLength(500);

        e.HasQueryFilter(x => x.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin);

        e.HasIndex(x => x.TenantId);
        e.HasIndex(x => new { x.TenantId, x.Name }).IsUnique();
    }
}