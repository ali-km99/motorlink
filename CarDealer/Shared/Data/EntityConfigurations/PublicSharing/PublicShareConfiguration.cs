using CarDealer.API.Features.PublicSharing.Entities;
using CarDealer.API.Shared.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarDealer.API.Shared.Data.EntityConfigurations.PublicSharing;

public class PublicShareConfiguration : IEntityTypeConfiguration<PublicShare>
{
    private readonly ICurrentTenantService _currentTenant;

    public PublicShareConfiguration(ICurrentTenantService currentTenant)
    {
        _currentTenant = currentTenant;
    }

    public void Configure(EntityTypeBuilder<PublicShare> e)
    {
        e.HasKey(x => x.Id);
        e.Property(x => x.Token).HasMaxLength(64).IsRequired();
        e.Property(x => x.ContactAddress).HasMaxLength(300);

        e.HasIndex(x => x.Token).IsUnique();

        e.HasOne(x => x.Car)
         .WithMany()
         .HasForeignKey(x => x.CarId)
         .OnDelete(DeleteBehavior.Cascade)
         .IsRequired(false);

        e.HasQueryFilter(x => x.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin);

        e.HasIndex(x => x.CarId);
    }
}