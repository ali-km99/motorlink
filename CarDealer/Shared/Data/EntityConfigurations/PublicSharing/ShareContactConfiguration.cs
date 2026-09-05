using CarDealer.API.Features.PublicSharing.Entities;
using CarDealer.API.Shared.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarDealer.API.Shared.Data.EntityConfigurations.PublicSharing;

public class ShareContactConfiguration : IEntityTypeConfiguration<ShareContact>
{
    private readonly ICurrentTenantService _currentTenant;

    public ShareContactConfiguration(ICurrentTenantService currentTenant)
    {
        _currentTenant = currentTenant;
    }

    public void Configure(EntityTypeBuilder<ShareContact> e)
    {
        e.HasKey(x => x.Id);
        e.Property(x => x.Label).HasMaxLength(50).IsRequired();
        e.Property(x => x.Value).HasMaxLength(50).IsRequired();

        e.HasOne(x => x.Share)
         .WithMany(s => s.Contacts)
         .HasForeignKey(x => x.ShareId)
         .OnDelete(DeleteBehavior.Cascade);

        e.HasQueryFilter(x => x.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin);

        e.HasIndex(x => x.ShareId);
    }
}