using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarDealer.API.Migrations
{
    /// <inheritdoc />
    public partial class FillTenantIdWithDefaultTenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Fill TenantId = 1 (default tenant) on all existing records
            // Order matters: parent tables first (Tenant FK dependencies), then children

            migrationBuilder.Sql("UPDATE [Users] SET [TenantId] = 1 WHERE [TenantId] IS NULL");
            migrationBuilder.Sql("UPDATE [UserPermissions] SET [TenantId] = 1 WHERE [TenantId] IS NULL");
            migrationBuilder.Sql("UPDATE [Customers] SET [TenantId] = 1 WHERE [TenantId] IS NULL");
            migrationBuilder.Sql("UPDATE [Cars] SET [TenantId] = 1 WHERE [TenantId] IS NULL");
            migrationBuilder.Sql("UPDATE [CarImages] SET [TenantId] = 1 WHERE [TenantId] IS NULL");
            migrationBuilder.Sql("UPDATE [CarFeatures] SET [TenantId] = 1 WHERE [TenantId] IS NULL");
            migrationBuilder.Sql("UPDATE [CarComments] SET [TenantId] = 1 WHERE [TenantId] IS NULL");
            migrationBuilder.Sql("UPDATE [Maintenances] SET [TenantId] = 1 WHERE [TenantId] IS NULL");
            migrationBuilder.Sql("UPDATE [MaintenanceCenters] SET [TenantId] = 1 WHERE [TenantId] IS NULL");
            migrationBuilder.Sql("UPDATE [MaintenanceCenterPhones] SET [TenantId] = 1 WHERE [TenantId] IS NULL");
            migrationBuilder.Sql("UPDATE [MaintenancePayments] SET [TenantId] = 1 WHERE [TenantId] IS NULL");
            migrationBuilder.Sql("UPDATE [Sales] SET [TenantId] = 1 WHERE [TenantId] IS NULL");
            migrationBuilder.Sql("UPDATE [Transactions] SET [TenantId] = 1 WHERE [TenantId] IS NULL");
            migrationBuilder.Sql("UPDATE [PublicShares] SET [TenantId] = 1 WHERE [TenantId] IS NULL");
            migrationBuilder.Sql("UPDATE [ShareViews] SET [TenantId] = 1 WHERE [TenantId] IS NULL");
            migrationBuilder.Sql("UPDATE [ShareContacts] SET [TenantId] = 1 WHERE [TenantId] IS NULL");
            migrationBuilder.Sql("UPDATE [Expenses] SET [TenantId] = 1 WHERE [TenantId] IS NULL");
            migrationBuilder.Sql("UPDATE [ExpenseCategories] SET [TenantId] = 1 WHERE [TenantId] IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert: set TenantId back to NULL for rollback
            migrationBuilder.Sql("UPDATE [Users] SET [TenantId] = NULL");
            migrationBuilder.Sql("UPDATE [UserPermissions] SET [TenantId] = NULL");
            migrationBuilder.Sql("UPDATE [Customers] SET [TenantId] = NULL");
            migrationBuilder.Sql("UPDATE [Cars] SET [TenantId] = NULL");
            migrationBuilder.Sql("UPDATE [CarImages] SET [TenantId] = NULL");
            migrationBuilder.Sql("UPDATE [CarFeatures] SET [TenantId] = NULL");
            migrationBuilder.Sql("UPDATE [CarComments] SET [TenantId] = NULL");
            migrationBuilder.Sql("UPDATE [Maintenances] SET [TenantId] = NULL");
            migrationBuilder.Sql("UPDATE [MaintenanceCenters] SET [TenantId] = NULL");
            migrationBuilder.Sql("UPDATE [MaintenanceCenterPhones] SET [TenantId] = NULL");
            migrationBuilder.Sql("UPDATE [MaintenancePayments] SET [TenantId] = NULL");
            migrationBuilder.Sql("UPDATE [Sales] SET [TenantId] = NULL");
            migrationBuilder.Sql("UPDATE [Transactions] SET [TenantId] = NULL");
            migrationBuilder.Sql("UPDATE [PublicShares] SET [TenantId] = NULL");
            migrationBuilder.Sql("UPDATE [ShareViews] SET [TenantId] = NULL");
            migrationBuilder.Sql("UPDATE [ShareContacts] SET [TenantId] = NULL");
            migrationBuilder.Sql("UPDATE [Expenses] SET [TenantId] = NULL");
            migrationBuilder.Sql("UPDATE [ExpenseCategories] SET [TenantId] = NULL");
        }
    }
}
