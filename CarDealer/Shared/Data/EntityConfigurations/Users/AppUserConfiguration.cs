using CarDealer.API.Features.Users.Entities;
using CarDealer.API.Shared.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarDealer.API.Shared.Data.EntityConfigurations.Users;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    private readonly ICurrentTenantService _currentTenant;

    public AppUserConfiguration(ICurrentTenantService currentTenant)
    {
        _currentTenant = currentTenant;
    }

    public void Configure(EntityTypeBuilder<AppUser> e)
    {
        e.HasKey(x => x.Id);
        e.Property(x => x.Username).HasMaxLength(100).IsRequired();
        e.Property(x => x.Email).HasMaxLength(200).IsRequired();
        e.Property(x => x.PasswordHash).IsRequired();
        e.Property(x => x.Role).HasMaxLength(50).IsRequired();
        e.Property(x => x.RefreshToken).HasMaxLength(500);

        e.HasIndex(x => x.Email).IsUnique();
        e.HasIndex(x => x.Username).IsUnique();
        e.HasIndex(x => x.TenantId);
        e.HasIndex(x => x.IsPlatformAdmin);

        // ⚠️ حرج: هذا الفلتر يمنع Login/RegisterDealership من إيجاد أي مستخدم
        // ما لم تُستخدم .IgnoreQueryFilters() صراحةً بكل استعلام مجهول (قبل معرفة Tenant).
        e.HasQueryFilter(x => x.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin);
    }
}