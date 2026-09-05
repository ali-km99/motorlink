using CarDealer.API.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarDealer.API.Shared.Data.EntityConfigurations.Platform;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> e)
    {
        e.HasKey(x => x.Id);
        e.Property(x => x.Name).HasMaxLength(200).IsRequired();
        e.Property(x => x.Slug).HasMaxLength(100).IsRequired();
        e.HasIndex(x => x.Slug).IsUnique();
        e.HasIndex(x => x.IsActive);

        // Seed the default tenant that will own all existing data after migration.
        e.HasData(new Tenant
        {
            Id = 1,
            Name = "Default Dealer",
            Slug = "default",
            IsActive = true,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}