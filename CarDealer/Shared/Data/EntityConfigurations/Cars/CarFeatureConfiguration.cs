using CarDealer.API.Features.Cars.Entities;
using CarDealer.API.Shared.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarDealer.API.Shared.Data.EntityConfigurations.Cars;

public class CarFeatureConfiguration : IEntityTypeConfiguration<CarFeature>
{
    private readonly ICurrentTenantService _currentTenant;

    public CarFeatureConfiguration(ICurrentTenantService currentTenant)
    {
        _currentTenant = currentTenant;
    }

    public void Configure(EntityTypeBuilder<CarFeature> e)
    {
        e.HasKey(x => x.Id);

        e.HasOne(x => x.Car)
         .WithMany(x => x.CarFeatures)
         .HasForeignKey(x => x.CarId)
         .OnDelete(DeleteBehavior.Cascade)
         .IsRequired(false);

        e.HasOne(x => x.Feature)
         .WithMany(x => x.CarFeatures)
         .HasForeignKey(x => x.FeatureId)
         .OnDelete(DeleteBehavior.Restrict);

        e.HasQueryFilter(x => x.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin);

        e.HasIndex(x => new { x.CarId, x.FeatureId }).IsUnique();
    }
}