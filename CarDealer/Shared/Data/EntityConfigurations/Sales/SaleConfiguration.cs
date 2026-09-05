using CarDealer.API.Features.Sales.Entities;
using CarDealer.API.Shared.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarDealer.API.Shared.Data.EntityConfigurations.Sales;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    private readonly ICurrentTenantService _currentTenant;

    public SaleConfiguration(ICurrentTenantService currentTenant)
    {
        _currentTenant = currentTenant;
    }

    public void Configure(EntityTypeBuilder<Sale> e)
    {
        e.HasKey(x => x.Id);
        e.Property(x => x.SoldPrice).HasColumnType("decimal(18,2)");

        e.HasOne(x => x.Car)
         .WithOne(x => x.Sale)
         .HasForeignKey<Sale>(x => x.CarId)
         .OnDelete(DeleteBehavior.Restrict)
         .IsRequired(false);

        e.HasOne(x => x.Customer)
         .WithMany(x => x.Sales)
         .HasForeignKey(x => x.CustomerId)
         .OnDelete(DeleteBehavior.Restrict)
         .IsRequired(false);

        e.HasQueryFilter(x => x.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin);

        e.HasIndex(x => x.CarId).IsUnique();
        e.HasIndex(x => x.CustomerId);
        e.HasIndex(x => x.SoldDate);
    }
}