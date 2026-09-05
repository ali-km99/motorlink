using CarDealer.API.Features.Expenses.Entities;
using CarDealer.API.Shared.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarDealer.API.Shared.Data.EntityConfigurations.Expenses;

public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    private readonly ICurrentTenantService _currentTenant;

    public ExpenseConfiguration(ICurrentTenantService currentTenant)
    {
        _currentTenant = currentTenant;
    }

    public void Configure(EntityTypeBuilder<Expense> e)
    {
        e.HasKey(x => x.Id);
        e.Property(x => x.Amount).HasColumnType("decimal(18,2)");
        e.Property(x => x.Description).HasMaxLength(500);

        e.HasOne(x => x.Category)
         .WithMany(c => c.Expenses)
         .HasForeignKey(x => x.CategoryId)
         .OnDelete(DeleteBehavior.Restrict); // منع حذف تصنيف مستخدم في مصروفات

        e.HasQueryFilter(x => !x.IsDeleted
            && (x.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin));

        e.HasIndex(x => x.CategoryId);
        e.HasIndex(x => x.TenantId);
        e.HasIndex(x => x.Date);
    }
}