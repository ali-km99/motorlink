using CarDealer.API.Features.Cars.Entities;
using CarDealer.API.Shared.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarDealer.API.Shared.Data.EntityConfigurations.Cars;

public class CarConfiguration : IEntityTypeConfiguration<Car>
{
    private readonly ICurrentTenantService _currentTenant;

    public CarConfiguration(ICurrentTenantService currentTenant)
    {
        _currentTenant = currentTenant;
    }

    public void Configure(EntityTypeBuilder<Car> e)
    {
        e.HasKey(x => x.Id);
        e.Property(x => x.Brand).HasMaxLength(100).IsRequired();
        e.Property(x => x.Model).HasMaxLength(100).IsRequired();
        e.Property(x => x.ExteriorColor).HasMaxLength(50);
        e.Property(x => x.InteriorColor).HasMaxLength(50);
        e.Property(x => x.CostPrice).HasColumnType("decimal(18,2)");
        e.Property(x => x.ShippingCost).HasColumnType("decimal(18,2)");
        e.Property(x => x.SellingPrice).HasColumnType("decimal(18,2)");
        e.Property(x => x.DiscountedPrice).HasColumnType("decimal(18,2)");
        e.Property(x => x.VinNumber).HasMaxLength(17);
        e.Property(x => x.MileageUnit).HasMaxLength(5);
        e.Property(x => x.BodyType).HasMaxLength(50);
        e.Property(x => x.Transmission).HasMaxLength(50);
        e.Property(x => x.Condition).HasMaxLength(50);
        e.Property(x => x.FuelType).HasMaxLength(50);
        e.Property(x => x.Specs).HasMaxLength(50);
        e.Property(x => x.BodyCondition).HasMaxLength(50);
        e.Property(x => x.PaymentMethod).HasMaxLength(50);

        e.HasOne(x => x.Status)
         .WithMany(x => x.Cars)
         .HasForeignKey(x => x.StatusId)
         .OnDelete(DeleteBehavior.Restrict);

        e.HasQueryFilter(x => !x.IsDeleted
            && (x.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin));

        e.HasIndex(x => x.StatusId);
        e.HasIndex(x => x.IsDeleted);
        e.HasIndex(x => x.Brand);
        e.HasIndex(x => x.Year);
        e.HasIndex(x => x.TenantId);
        e.HasIndex(x => new { x.TenantId, x.VinNumber })
            .IsUnique().HasFilter("[VinNumber] IS NOT NULL");
    }
}