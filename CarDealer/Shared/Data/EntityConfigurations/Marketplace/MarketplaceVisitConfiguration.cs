using CarDealer.API.Features.Marketplace.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarDealer.API.Shared.Data.EntityConfigurations.Marketplace;

public class MarketplaceVisitConfiguration : IEntityTypeConfiguration<MarketplaceVisit>
{
    public void Configure(EntityTypeBuilder<MarketplaceVisit> e)
    {
        e.HasKey(x => x.Id);

        e.HasOne(x => x.MarketplaceUser)
         .WithMany()
         .HasForeignKey(x => x.MarketplaceUserId)
         .OnDelete(DeleteBehavior.SetNull);

        e.HasIndex(x => x.VisitedAt);
        e.HasIndex(x => x.MarketplaceUserId);
    }
}