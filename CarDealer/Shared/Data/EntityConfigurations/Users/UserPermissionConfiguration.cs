using CarDealer.API.Features.Users.Entities;
using CarDealer.API.Shared.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarDealer.API.Shared.Data.EntityConfigurations.Users;

public class UserPermissionConfiguration : IEntityTypeConfiguration<UserPermission>
{
    private readonly ICurrentTenantService _currentTenant;

    public UserPermissionConfiguration(ICurrentTenantService currentTenant)
    {
        _currentTenant = currentTenant;
    }

    public void Configure(EntityTypeBuilder<UserPermission> e)
    {
        e.HasKey(x => x.Id);

        e.HasOne(x => x.User)
         .WithMany(u => u.Permissions)
         .HasForeignKey(x => x.UserId)
         .OnDelete(DeleteBehavior.Cascade);

        e.HasOne(x => x.Permission)
         .WithMany(p => p.UserPermissions)
         .HasForeignKey(x => x.PermissionId)
         .OnDelete(DeleteBehavior.Cascade);

        e.HasQueryFilter(x => x.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin);

        e.HasIndex(x => new { x.UserId, x.PermissionId }).IsUnique();
    }
}