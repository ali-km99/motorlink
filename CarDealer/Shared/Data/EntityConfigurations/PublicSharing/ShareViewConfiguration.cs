using CarDealer.API.Features.PublicSharing.Entities;
using CarDealer.API.Shared.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarDealer.API.Shared.Data.EntityConfigurations.PublicSharing;

public class ShareViewConfiguration : IEntityTypeConfiguration<ShareView>
{
    private readonly ICurrentTenantService _currentTenant;

    public ShareViewConfiguration(ICurrentTenantService currentTenant)
    {
        _currentTenant = currentTenant;
    }

    public void Configure(EntityTypeBuilder<ShareView> e)
    {
        e.HasKey(x => x.Id);

        e.HasOne(x => x.Share)
         .WithMany(s => s.Views)
         .HasForeignKey(x => x.ShareId)
         .OnDelete(DeleteBehavior.Cascade);

        e.HasQueryFilter(x => x.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin);

        e.HasIndex(x => x.ShareId);
    }
}