using CarDealer.API.Features.Marketplace.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarDealer.API.Shared.Data.EntityConfigurations.Marketplace;

public class MarketplaceUserConfiguration : IEntityTypeConfiguration<MarketplaceUser>
{
    public void Configure(EntityTypeBuilder<MarketplaceUser> e)
    {
        e.HasKey(x => x.Id);
        e.Property(x => x.Username).HasMaxLength(100).IsRequired();
        e.Property(x => x.Email).HasMaxLength(200).IsRequired();
        e.Property(x => x.PasswordHash).IsRequired();
        e.Property(x => x.RefreshToken).HasMaxLength(500);

        e.HasIndex(x => x.Email).IsUnique();
        e.HasIndex(x => x.Username).IsUnique();
    }
}