using CarDealer.API.Features.Transactions.Entities;
using CarDealer.API.Shared.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarDealer.API.Shared.Data.EntityConfigurations.Transactions;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    private readonly ICurrentTenantService _currentTenant;

    public TransactionConfiguration(ICurrentTenantService currentTenant)
    {
        _currentTenant = currentTenant;
    }

    public void Configure(EntityTypeBuilder<Transaction> e)
    {
        e.HasKey(x => x.Id);
        e.Property(x => x.Type).HasMaxLength(50).IsRequired();
        e.Property(x => x.Amount).HasColumnType("decimal(18,2)");
        e.Property(x => x.RelatedEntity).HasMaxLength(50);

        e.HasIndex(x => x.Type);
        e.HasIndex(x => x.TenantId);
        e.HasIndex(x => x.Date);

        e.HasQueryFilter(x => !x.IsDeleted
            && (x.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin));
    }
}