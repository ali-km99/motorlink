using CarDealer.API.Features.Users.Entities;
using CarDealer.API.Shared.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarDealer.API.Shared.Data.EntityConfigurations.Users;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> e)
    {
        e.HasKey(x => x.Id);
        e.Property(x => x.Code).HasMaxLength(100).IsRequired();
        e.Property(x => x.Name).HasMaxLength(150).IsRequired();
        e.Property(x => x.Category).HasMaxLength(50).IsRequired();
        e.HasIndex(x => x.Code).IsUnique();
        // عالمي — لا TenantId، لا فلتر

        // Seed كامل قائمة الصلاحيات بالنظام
        e.HasData(
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