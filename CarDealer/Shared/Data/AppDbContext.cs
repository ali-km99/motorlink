using CarDealer.API.Features.Cars.Entities;
using CarDealer.API.Features.Customers.Entities;
using CarDealer.API.Features.Expenses.Entities;
using CarDealer.API.Features.Maintenance.Entities;
using CarDealer.API.Features.Marketplace.Entities;
using CarDealer.API.Features.Platform.Entities;
using CarDealer.API.Features.PublicSharing.Entities;
using CarDealer.API.Features.Sales.Entities;
using CarDealer.API.Features.Transactions.Entities;
using CarDealer.API.Features.Users.Entities;
using CarDealer.API.Shared.Data.EntityConfigurations.Cars;
using CarDealer.API.Shared.Data.EntityConfigurations.Customers;
using CarDealer.API.Shared.Data.EntityConfigurations.Expenses;
using CarDealer.API.Shared.Data.EntityConfigurations.Maintenance;
using CarDealer.API.Shared.Data.EntityConfigurations.Marketplace;
using CarDealer.API.Shared.Data.EntityConfigurations.Platform;
using CarDealer.API.Shared.Data.EntityConfigurations.PublicSharing;
using CarDealer.API.Shared.Data.EntityConfigurations.Sales;
using CarDealer.API.Shared.Data.EntityConfigurations.Transactions;
using CarDealer.API.Shared.Data.EntityConfigurations.Users;
using CarDealer.API.Shared.Entities;
using CarDealer.API.Shared.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CarDealer.API.Shared.Data;

public class AppDbContext : DbContext
{
    private readonly ICurrentTenantService _currentTenant;

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentTenantService currentTenant)
        : base(options)
    {
        _currentTenant = currentTenant;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Car> Cars => Set<Car>();
    public DbSet<CarStatus> CarStatuses => Set<CarStatus>();
    public DbSet<CarImage> CarImages => Set<CarImage>();
    public DbSet<CarComment> CarComments => Set<CarComment>();
    public DbSet<Feature> Features => Set<Feature>();
    public DbSet<CarFeature> CarFeatures => Set<CarFeature>();
    public DbSet<MaintenanceEntity> Maintenances => Set<MaintenanceEntity>();
    public DbSet<MaintenanceCenter> MaintenanceCenters => Set<MaintenanceCenter>();
    public DbSet<MaintenanceCenterPhone> MaintenanceCenterPhones => Set<MaintenanceCenterPhone>();
    public DbSet<MaintenancePayment> MaintenancePayments => Set<MaintenancePayment>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserPermission> UserPermissions => Set<UserPermission>();
    public DbSet<PublicShare> PublicShares => Set<PublicShare>();
    public DbSet<ShareView> ShareViews => Set<ShareView>();
    public DbSet<ShareContact> ShareContacts => Set<ShareContact>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<ExpenseCategory> ExpenseCategories => Set<ExpenseCategory>();
    public DbSet<MarketplaceUser> MarketplaceUsers => Set<MarketplaceUser>();
    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    public DbSet<TenantSubscription> TenantSubscriptions => Set<TenantSubscription>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ─── جداول Tenant-scoped (تحتاج ICurrentTenantService للفلتر) ───────────
        modelBuilder.ApplyConfiguration(new CarConfiguration(_currentTenant));
        modelBuilder.ApplyConfiguration(new CarImageConfiguration(_currentTenant));
        modelBuilder.ApplyConfiguration(new CarCommentConfiguration(_currentTenant));
        modelBuilder.ApplyConfiguration(new CarFeatureConfiguration(_currentTenant));
        modelBuilder.ApplyConfiguration(new MaintenanceCenterConfiguration(_currentTenant));
        modelBuilder.ApplyConfiguration(new MaintenanceCenterPhoneConfiguration(_currentTenant));
        modelBuilder.ApplyConfiguration(new MaintenanceConfiguration(_currentTenant));
        modelBuilder.ApplyConfiguration(new MaintenancePaymentConfiguration(_currentTenant));
        modelBuilder.ApplyConfiguration(new CustomerConfiguration(_currentTenant));
        modelBuilder.ApplyConfiguration(new SaleConfiguration(_currentTenant));
        modelBuilder.ApplyConfiguration(new TransactionConfiguration(_currentTenant));
        modelBuilder.ApplyConfiguration(new AppUserConfiguration(_currentTenant));
        modelBuilder.ApplyConfiguration(new UserPermissionConfiguration(_currentTenant));
        modelBuilder.ApplyConfiguration(new PublicShareConfiguration(_currentTenant));
        modelBuilder.ApplyConfiguration(new ShareContactConfiguration(_currentTenant));
        modelBuilder.ApplyConfiguration(new ShareViewConfiguration(_currentTenant));
        modelBuilder.ApplyConfiguration(new ExpenseCategoryConfiguration(_currentTenant));
        modelBuilder.ApplyConfiguration(new ExpenseConfiguration(_currentTenant));

        // ─── جداول عالمية (بلا فلتر Tenant) ─────────────────────────────────────
        modelBuilder.ApplyConfiguration(new TenantConfiguration());
        modelBuilder.ApplyConfiguration(new CarStatusConfiguration());
        modelBuilder.ApplyConfiguration(new FeatureConfiguration());
        modelBuilder.ApplyConfiguration(new PermissionConfiguration());
        modelBuilder.ApplyConfiguration(new MarketplaceUserConfiguration());
        modelBuilder.ApplyConfiguration(new SubscriptionPlanConfiguration());
        modelBuilder.ApplyConfiguration(new TenantSubscriptionConfiguration());
    }
}