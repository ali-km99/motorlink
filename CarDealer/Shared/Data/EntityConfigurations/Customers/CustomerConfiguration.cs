using CarDealer.API.Features.Customers.Entities;
using CarDealer.API.Shared.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarDealer.API.Shared.Data.EntityConfigurations.Customers;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    private readonly ICurrentTenantService _currentTenant;

    public CustomerConfiguration(ICurrentTenantService currentTenant)
    {
        _currentTenant = currentTenant;
    }

    public void Configure(EntityTypeBuilder<Customer> e)
    {
        e.HasKey(x => x.Id);
        e.Property(x => x.Name).HasMaxLength(150).IsRequired();
        e.Property(x => x.Phone).HasMaxLength(50).IsRequired();

        e.HasQueryFilter(x => !x.IsDeleted
            && (x.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin));

        e.HasIndex(x => x.TenantId);
    }
}