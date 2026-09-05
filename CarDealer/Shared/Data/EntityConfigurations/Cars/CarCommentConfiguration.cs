using CarDealer.API.Features.Cars.Entities;
using CarDealer.API.Shared.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarDealer.API.Shared.Data.EntityConfigurations.Cars;

public class CarCommentConfiguration : IEntityTypeConfiguration<CarComment>
{
    private readonly ICurrentTenantService _currentTenant;

    public CarCommentConfiguration(ICurrentTenantService currentTenant)
    {
        _currentTenant = currentTenant;
    }

    public void Configure(EntityTypeBuilder<CarComment> e)
    {
        e.HasKey(x => x.Id);
        e.Property(x => x.Body).HasMaxLength(2000).IsRequired();

        e.HasOne(x => x.Car)
         .WithMany(x => x.Comments)
         .HasForeignKey(x => x.CarId)
         .OnDelete(DeleteBehavior.Cascade)
         .IsRequired(false);

        e.HasOne(x => x.AuthorUser)
         .WithMany(u => u.Comments)
         .HasForeignKey(x => x.AuthorUserId)
         .OnDelete(DeleteBehavior.SetNull)
         .IsRequired(false);

        e.HasQueryFilter(x => !x.IsDeleted
            && (x.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin));

        e.HasIndex(x => x.CarId);
        e.HasIndex(x => x.TenantId);
    }
}