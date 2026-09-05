using CarDealer.API.Common;
using CarDealer.API.Entities;
using CarDealer.API.Features.Cars.Entities;
using CarDealer.API.Features.Customers.Entities;
using CarDealer.API.Features.Expenses.Entities;
using CarDealer.API.Features.Maintenance.Entities;
using CarDealer.API.Features.Marketplace.Entities;
using CarDealer.API.Features.PublicSharing.Entities;
using CarDealer.API.Features.Sales.Entities;
using CarDealer.API.Features.Transactions.Entities;
using CarDealer.API.Features.Users.Entities;
using CarDealer.API.Services;

using Microsoft.EntityFrameworkCore;

namespace CarDealer.API.Data;

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

        // ─── Tenant (عالمي — لا فلتر Tenant عليه هو نفسه) ────────────────────
        modelBuilder.Entity<Tenant>(e =>
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
        });

        // ─── Car ───────────────────────────────────────────────────────────
        modelBuilder.Entity<Car>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Brand).HasMaxLength(100).IsRequired();
            e.Property(x => x.Model).HasMaxLength(100).IsRequired();
            e.Property(x => x.ExteriorColor).HasMaxLength(50);
            e.Property(x => x.InteriorColor).HasMaxLength(50);
            e.Property(x => x.CostPrice).HasColumnType("decimal(18,2)");
            e.Property(x => x.ShippingCost).HasColumnType("decimal(18,2)");
            e.Property(x => x.SellingPrice).HasColumnType("decimal(18,2)");
            e.Property(x => x.DiscountedPrice).HasColumnType("decimal(18,2)");
            e.Property(x => x.VinNumber).HasMaxLength(17);
            e.Property(x => x.MileageUnit).HasMaxLength(5);
            e.Property(x => x.BodyType).HasMaxLength(50);
            e.Property(x => x.Transmission).HasMaxLength(50);
            e.Property(x => x.Condition).HasMaxLength(50);
            e.Property(x => x.FuelType).HasMaxLength(50);
            e.Property(x => x.Specs).HasMaxLength(50);
            e.Property(x => x.BodyCondition).HasMaxLength(50);
            e.Property(x => x.PaymentMethod).HasMaxLength(50);

            e.HasOne(x => x.Status)
             .WithMany(x => x.Cars)
             .HasForeignKey(x => x.StatusId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasQueryFilter(x => !x.IsDeleted
                && (x.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin));

            e.HasIndex(x => x.StatusId);
            e.HasIndex(x => x.IsDeleted);
            e.HasIndex(x => x.Brand);
            e.HasIndex(x => x.Year);
            e.HasIndex(x => x.TenantId);
            e.HasIndex(x => new { x.TenantId, x.VinNumber })
                .IsUnique().HasFilter("[VinNumber] IS NOT NULL");
        });

        // ─── CarImage ──────────────────────────────────────────────────────
        modelBuilder.Entity<CarImage>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.ImageUrl).HasMaxLength(500).IsRequired();

            e.HasOne(x => x.Car)
             .WithMany(x => x.Images)
             .HasForeignKey(x => x.CarId)
             .OnDelete(DeleteBehavior.Cascade)
             .IsRequired(false);

            e.HasQueryFilter(x => x.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin);

            e.HasIndex(x => x.CarId);
        });

        // ─── CarComment (Phase 1: new table) ──────────────────────────────
        modelBuilder.Entity<CarComment>(e =>
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
        });


        modelBuilder.Entity<Feature>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Category).HasMaxLength(50).IsRequired();
            e.HasIndex(x => x.Category);
            // عالمي — لا TenantId، لا فلتر
        });


        // ─── CarFeature ────────────────────────────────────────────────────
        modelBuilder.Entity<CarFeature>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Car)
             .WithMany(x => x.CarFeatures)
             .HasForeignKey(x => x.CarId)
             .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
            e.HasOne(x => x.Feature)
             .WithMany(x => x.CarFeatures)
             .HasForeignKey(x => x.FeatureId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasQueryFilter(x => x.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin);

            e.HasIndex(x => new { x.CarId, x.FeatureId }).IsUnique();
        });

        // ─── MaintenanceCenter ─────────────────────────────────────────────
        modelBuilder.Entity<MaintenanceCenter>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Notes).HasMaxLength(500);

            e.HasQueryFilter(x => x.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin);

            e.HasIndex(x => x.TenantId);
            e.HasIndex(x => new { x.TenantId, x.Name }).IsUnique();
        });

        // ─── MaintenanceCenterPhone ──────────────────────────────────────────
        modelBuilder.Entity<MaintenanceCenterPhone>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Label).HasMaxLength(50).IsRequired();
            e.Property(x => x.PhoneNumber).HasMaxLength(50).IsRequired();

            e.HasOne(x => x.MaintenanceCenter)
             .WithMany(c => c.Phones)
             .HasForeignKey(x => x.MaintenanceCenterId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasQueryFilter(x => x.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin);

            e.HasIndex(x => x.MaintenanceCenterId);
        });

        // ─── Maintenance ───────────────────────────────────────────────────
        modelBuilder.Entity<MaintenanceEntity>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.IssueDescription).HasMaxLength(500).IsRequired();
            e.Property(x => x.RepairCost).HasColumnType("decimal(18,2)");

            e.HasOne(x => x.Car)
             .WithMany(x => x.Maintenances)
             .HasForeignKey(x => x.CarId)
             .OnDelete(DeleteBehavior.Restrict)
             .IsRequired(false);

            e.HasOne(x => x.MaintenanceCenter)
             .WithMany(x => x.Maintenances)
             .HasForeignKey(x => x.MaintenanceCenterId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasQueryFilter(x => x.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin);

            e.HasIndex(x => x.CarId);
            e.HasIndex(x => x.MaintenanceCenterId);
            e.HasIndex(x => new { x.MaintenanceCenterId, x.CreatedAt });
        });

        // ─── MaintenancePayment ────────────────────────────────────────────
        modelBuilder.Entity<MaintenancePayment>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            e.Property(x => x.Notes).HasMaxLength(500);

            e.HasOne(x => x.Maintenance)
             .WithMany(x => x.Payments)
             .HasForeignKey(x => x.MaintenanceId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasQueryFilter(x => x.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin);

            e.HasIndex(x => x.MaintenanceId);
            e.HasIndex(x => x.PaymentDate);
        });

        // ─── Customer ──────────────────────────────────────────────────────
        modelBuilder.Entity<Customer>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(150).IsRequired();
            e.Property(x => x.Phone).HasMaxLength(50).IsRequired();

            e.HasQueryFilter(x => !x.IsDeleted
                && (x.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin));

            e.HasIndex(x => x.TenantId);
        });

        // ─── Sale ──────────────────────────────────────────────────────────
        modelBuilder.Entity<Sale>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SoldPrice).HasColumnType("decimal(18,2)");

            e.HasOne(x => x.Car)
             .WithOne(x => x.Sale)
             .HasForeignKey<Sale>(x => x.CarId)
             .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
            e.HasOne(x => x.Customer)
             .WithMany(x => x.Sales)
             .HasForeignKey(x => x.CustomerId)
             .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

            e.HasQueryFilter(x => x.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin);

            e.HasIndex(x => x.CarId).IsUnique();
            e.HasIndex(x => x.CustomerId);
            e.HasIndex(x => x.SoldDate);
        });

        // ─── Transaction ───────────────────────────────────────────────────
        modelBuilder.Entity<Transaction>(e =>
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
        });

        // ─── AppUser ───────────────────────────────────────────────────────
        modelBuilder.Entity<AppUser>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Username).HasMaxLength(100).IsRequired();
            e.Property(x => x.Email).HasMaxLength(200).IsRequired();
            e.Property(x => x.PasswordHash).IsRequired();
            e.Property(x => x.Role).HasMaxLength(50).IsRequired();
            e.Property(x => x.RefreshToken).HasMaxLength(500);
            e.HasIndex(x => x.Email).IsUnique();
            e.HasIndex(x => x.TenantId);
            e.HasIndex(x => x.IsPlatformAdmin);
            e.HasIndex(x => x.Username).IsUnique();

            // ⚠️ حرج: هذا الفلتر يمنع Login/RegisterDealership من إيجاد أي مستخدم
            // ما لم تُستخدم .IgnoreQueryFilters() صراحة بكل استعلام مجهول (قبل معرفة Tenant).
            e.HasQueryFilter(x => x.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin);
        });

        // ─── MarketplaceUser (منفصل كلياً — لا TenantId، لا فلتر) ─────────
        modelBuilder.Entity<MarketplaceUser>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Username).HasMaxLength(100).IsRequired();
            e.Property(x => x.Email).HasMaxLength(200).IsRequired();
            e.Property(x => x.PasswordHash).IsRequired();
            e.Property(x => x.RefreshToken).HasMaxLength(500);
            e.HasIndex(x => x.Email).IsUnique();
            e.HasIndex(x => x.Username).IsUnique();
        });

        // ─── Seed CarStatus (عالمي) ────────────────────────────────────────
        modelBuilder.Entity<CarStatus>().HasData(
            new CarStatus { Id = 1, Name = "Ready" },
            new CarStatus { Id = 2, Name = "Maintenance" },
            new CarStatus { Id = 3, Name = "Shipping" },
            new CarStatus { Id = 4, Name = "Sold" }
        );


        modelBuilder.Entity<Permission>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Code).HasMaxLength(100).IsRequired();
            e.Property(x => x.Name).HasMaxLength(150).IsRequired();
            e.Property(x => x.Category).HasMaxLength(50).IsRequired();
            e.HasIndex(x => x.Code).IsUnique();
            // عالمي — لا TenantId، لا فلتر
        });

        modelBuilder.Entity<UserPermission>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.User).WithMany(u => u.Permissions)
             .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Permission).WithMany(p => p.UserPermissions)
             .HasForeignKey(x => x.PermissionId).OnDelete(DeleteBehavior.Cascade);

            e.HasQueryFilter(x => x.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin);

            e.HasIndex(x => new { x.UserId, x.PermissionId }).IsUnique();
        });

        // ─── SubscriptionPlan (عالمي — لا TenantId، لا فلتر) ─────────────
        modelBuilder.Entity<SubscriptionPlan>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Code).HasMaxLength(50).IsRequired();
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.HasIndex(x => x.Code).IsUnique();

            e.HasData(
                new SubscriptionPlan
                {
                    Id = 1,
                    Code = "Basic",
                    Name = "الأساسية",
                    IsActive = true,
                    AllowMaintenanceDebtReports = false,
                    AllowPublicSharing = false,
                    AllowExpensesModule = false
                },
                new SubscriptionPlan
                {
                    Id = 2,
                    Code = "Professional",
                    Name = "الاحترافية",
                    IsActive = true,
                    AllowMaintenanceDebtReports = true,
                    AllowPublicSharing = true,
                    AllowExpensesModule = false
                },
                new SubscriptionPlan
                {
                    Id = 3,
                    Code = "Business",
                    Name = "الأعمال",
                    IsActive = true,
                    AllowMaintenanceDebtReports = true,
                    AllowPublicSharing = true,
                    AllowExpensesModule = true
                }
            );
        });

        // ─── TenantSubscription — عمداً بلا HasQueryFilter تلقائي ─────────
        // (يُدار من SuperAdmin عبر كل المعارض، أو يُفحص للمعرض الحالي بفلترة صريحة عبر ITenantFeatureService)
        modelBuilder.Entity<TenantSubscription>(e =>
        {
            e.HasKey(x => x.Id);

            e.HasOne(x => x.Tenant)
             .WithMany()
             .HasForeignKey(x => x.TenantId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.SubscriptionPlan)
             .WithMany(p => p.TenantSubscriptions)
             .HasForeignKey(x => x.SubscriptionPlanId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(x => x.TenantId);
            e.HasIndex(x => new { x.TenantId, x.IsActive });

            // Tenant الافتراضي (Id=1) يُربط تلقائياً بخطة Business — حتى لا تنكسر بيئة التطوير الحالية
            e.HasData(new TenantSubscription
            {
                Id = 1,
                TenantId = 1,
                SubscriptionPlanId = 3,
                IsActive = true,
                StartedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });
        });

        modelBuilder.Entity<PublicShare>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Token).HasMaxLength(64).IsRequired();
            e.Property(x => x.ContactAddress).HasMaxLength(300);
            e.HasIndex(x => x.Token).IsUnique();

            e.HasOne(x => x.Car)
             .WithMany()
             .HasForeignKey(x => x.CarId)
             .OnDelete(DeleteBehavior.Cascade)
             .IsRequired(false);

            e.HasQueryFilter(x => x.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin);

            e.HasIndex(x => x.CarId);
        });

        modelBuilder.Entity<ShareContact>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Label).HasMaxLength(50).IsRequired();
            e.Property(x => x.Value).HasMaxLength(50).IsRequired();

            e.HasOne(x => x.Share)
             .WithMany(s => s.Contacts)
             .HasForeignKey(x => x.ShareId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasQueryFilter(x => x.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin);

            e.HasIndex(x => x.ShareId);
        });

        modelBuilder.Entity<ShareView>(e =>
        {
            e.HasKey(x => x.Id);

            e.HasOne(x => x.Share)
             .WithMany(s => s.Views)
             .HasForeignKey(x => x.ShareId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasQueryFilter(x => x.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin);

            e.HasIndex(x => x.ShareId);
        });


        // ─── ExpenseCategory ───────────────────────────────────────────────
        modelBuilder.Entity<ExpenseCategory>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();

            e.HasQueryFilter(x => x.TenantId == _currentTenant.TenantId || _currentTenant.IsPlatformAdmin);

            e.HasIndex(x => x.TenantId);
            e.HasIndex(x => new { x.TenantId, x.Name }).IsUnique();
        });

        // ─── Expense ───────────────────────────────────────────────────────
        modelBuilder.Entity<Expense>(e =>
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
        });

        // Seed كل الصلاحيات الممكنة بالنظام
        modelBuilder.Entity<Permission>().HasData(
            new Permission { Id = 1, Code = PermissionCodes.CarsView, Name = "عرض السيارات", Category = "Cars" },
            new Permission { Id = 2, Code = PermissionCodes.CarsCreate, Name = "إضافة سيارة", Category = "Cars" },
            new Permission { Id = 3, Code = PermissionCodes.CarsUpdate, Name = "تعديل سيارة", Category = "Cars" },
            new Permission { Id = 4, Code = PermissionCodes.CarsDelete, Name = "حذف سيارة", Category = "Cars" },
            new Permission { Id = 5, Code = PermissionCodes.CarsUploadImages, Name = "رفع صور السيارة", Category = "Cars" },
            new Permission { Id = 6, Code = PermissionCodes.MaintenanceView, Name = "عرض الصيانات", Category = "Maintenance" },
            new Permission { Id = 7, Code = PermissionCodes.MaintenanceCreate, Name = "إضافة صيانة", Category = "Maintenance" },
            new Permission { Id = 8, Code = PermissionCodes.MaintenanceUpdate, Name = "تعديل صيانة", Category = "Maintenance" },
            new Permission { Id = 9, Code = PermissionCodes.MaintenanceDelete, Name = "حذف صيانة", Category = "Maintenance" },
            new Permission { Id = 10, Code = PermissionCodes.CustomersView, Name = "عرض العملاء", Category = "Customers" },
            new Permission { Id = 11, Code = PermissionCodes.CustomersCreate, Name = "إضافة عميل", Category = "Customers" },
            new Permission { Id = 16, Code = PermissionCodes.CustomersUpdate, Name = "تعديل عميل", Category = "Customers" },
            new Permission { Id = 17, Code = PermissionCodes.CustomersDelete, Name = "حذف عميل", Category = "Customers" },
            new Permission { Id = 12, Code = PermissionCodes.SalesView, Name = "عرض المبيعات", Category = "Sales" },
            new Permission { Id = 13, Code = PermissionCodes.SalesCreate, Name = "تسجيل عملية بيع", Category = "Sales" },
            new Permission { Id = 14, Code = PermissionCodes.TransactionsView, Name = "عرض المعاملات المالية", Category = "Transactions" },
            new Permission { Id = 18, Code = PermissionCodes.CarsShare, Name = "مشاركة ومتابعة روابط السيارات", Category = "Cars" },
            new Permission { Id = 19, Code = PermissionCodes.DashboardView, Name = "عرض لوحة التحكم", Category = "Dashboard" },
            new Permission { Id = 15, Code = PermissionCodes.UsersManage, Name = "إدارة المستخدمين والصلاحيات", Category = "Users" },
            new Permission { Id = 20, Code = PermissionCodes.ExpensesView, Name = "عرض المصروفات", Category = "Expenses" },
            new Permission { Id = 21, Code = PermissionCodes.ExpensesCreate, Name = "إضافة مصروف", Category = "Expenses" },
            new Permission { Id = 22, Code = PermissionCodes.ExpensesUpdate, Name = "تعديل مصروف", Category = "Expenses" },
            new Permission { Id = 23, Code = PermissionCodes.ExpensesDelete, Name = "حذف مصروف", Category = "Expenses" }
        );



    }
}