using CarDealer.API.Features.Cars.Entities;
using CarDealer.API.Shared.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarDealer.API.Shared.Data.EntityConfigurations.Cars;

public class CarImageConfiguration : IEntityTypeConfiguration<CarImage>
{
    private readonly ICurrentTenantService _currentTenant;

    public CarImageConfiguration(ICurrentTenantService currentTenant)
    {
        _currentTenant = currentTenant;
    }

    public void Configure(EntityTypeBuilder<CarImage> e)
    {
        e.HasKey(x => x.Id);
        e.Property(x => x.ImageUrl).HasMaxLength(500).IsRequired();

        e.HasOne(x => x.Car)
         .WithMany(x => x.Images)
         .HasForeignKey(x => x.CarId)
         .OnDelete(DeleteBehavior.Cascade)
         .IsRequired(false);

        // ملاحظة: CarImage لا يحتوي على IsDeleted — فقط فلتر Tenant.
        e.HasQueryFilter(x => x.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin);

        e.HasIndex(x => x.CarId);
    }
}