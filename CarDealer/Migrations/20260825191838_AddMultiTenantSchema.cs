using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarDealer.API.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiTenantSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPlatformAdmin",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "UserPermissions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "ShareViews",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "ShareContacts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PriceSource",
                table: "Sales",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Sales",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "PublicShares",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Maintenances",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "MaintenancePayments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "MaintenanceCenters",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "MaintenanceCenterPhones",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Expenses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "ExpenseCategories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Customers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DiscountEndAt",
                table: "Cars",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DiscountStartAt",
                table: "Cars",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountedPrice",
                table: "Cars",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HidePrice",
                table: "Cars",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDiscountActive",
                table: "Cars",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowContactCta",
                table: "Cars",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "Cars",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "CarImages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "CarFeatures",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CarComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarId = table.Column<int>(type: "int", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: true),
                    AuthorUserId = table.Column<int>(type: "int", nullable: true),
                    Body = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IsInternal = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarComments_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CarComments_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CarComments_Users_AuthorUserId",
                        column: x => x.AuthorUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "Tenants",
                columns: new[] { "Id", "CreatedAt", "IsActive", "Name", "Slug", "UpdatedAt" },
                values: new object[] { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Default Dealer", "default", null });

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsPlatformAdmin",
                table: "Users",
                column: "IsPlatformAdmin");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId",
                table: "Users",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_TenantId",
                table: "UserPermissions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TenantId",
                table: "Transactions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ShareViews_TenantId",
                table: "ShareViews",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ShareContacts_TenantId",
                table: "ShareContacts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_TenantId",
                table: "Sales",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PublicShares_TenantId",
                table: "PublicShares",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Maintenances_TenantId",
                table: "Maintenances",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePayments_TenantId",
                table: "MaintenancePayments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceCenters_TenantId",
                table: "MaintenanceCenters",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceCenterPhones_TenantId",
                table: "MaintenanceCenterPhones",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_TenantId",
                table: "Expenses",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseCategories_TenantId",
                table: "ExpenseCategories",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_TenantId",
                table: "Customers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_TenantId",
                table: "Cars",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CarImages_TenantId",
                table: "CarImages",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CarFeatures_TenantId",
                table: "CarFeatures",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CarComments_AuthorUserId",
                table: "CarComments",
                column: "AuthorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CarComments_CarId",
                table: "CarComments",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_CarComments_TenantId",
                table: "CarComments",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_IsActive",
                table: "Tenants",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Slug",
                table: "Tenants",
                column: "Slug",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CarFeatures_Tenants_TenantId",
                table: "CarFeatures",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CarImages_Tenants_TenantId",
                table: "CarImages",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_Tenants_TenantId",
                table: "Cars",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Tenants_TenantId",
                table: "Customers",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseCategories_Tenants_TenantId",
                table: "ExpenseCategories",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Tenants_TenantId",
                table: "Expenses",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceCenterPhones_Tenants_TenantId",
                table: "MaintenanceCenterPhones",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceCenters_Tenants_TenantId",
                table: "MaintenanceCenters",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenancePayments_Tenants_TenantId",
                table: "MaintenancePayments",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_Tenants_TenantId",
                table: "Maintenances",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PublicShares_Tenants_TenantId",
                table: "PublicShares",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Tenants_TenantId",
                table: "Sales",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShareContacts_Tenants_TenantId",
                table: "ShareContacts",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShareViews_Tenants_TenantId",
                table: "ShareViews",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Tenants_TenantId",
                table: "Transactions",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPermissions_Tenants_TenantId",
                table: "UserPermissions",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Tenants_TenantId",
                table: "Users",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarFeatures_Tenants_TenantId",
                table: "CarFeatures");

            migrationBuilder.DropForeignKey(
                name: "FK_CarImages_Tenants_TenantId",
                table: "CarImages");

            migrationBuilder.DropForeignKey(
                name: "FK_Cars_Tenants_TenantId",
                table: "Cars");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Tenants_TenantId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseCategories_Tenants_TenantId",
                table: "ExpenseCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Tenants_TenantId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceCenterPhones_Tenants_TenantId",
                table: "MaintenanceCenterPhones");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceCenters_Tenants_TenantId",
                table: "MaintenanceCenters");

            migrationBuilder.DropForeignKey(
                name: "FK_MaintenancePayments_Tenants_TenantId",
                table: "MaintenancePayments");

            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_Tenants_TenantId",
                table: "Maintenances");

            migrationBuilder.DropForeignKey(
                name: "FK_PublicShares_Tenants_TenantId",
                table: "PublicShares");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Tenants_TenantId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_ShareContacts_Tenants_TenantId",
                table: "ShareContacts");

            migrationBuilder.DropForeignKey(
                name: "FK_ShareViews_Tenants_TenantId",
                table: "ShareViews");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Tenants_TenantId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPermissions_Tenants_TenantId",
                table: "UserPermissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Tenants_TenantId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "CarComments");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Users_IsPlatformAdmin",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_TenantId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_UserPermissions_TenantId",
                table: "UserPermissions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_TenantId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_ShareViews_TenantId",
                table: "ShareViews");

            migrationBuilder.DropIndex(
                name: "IX_ShareContacts_TenantId",
                table: "ShareContacts");

            migrationBuilder.DropIndex(
                name: "IX_Sales_TenantId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_PublicShares_TenantId",
                table: "PublicShares");

            migrationBuilder.DropIndex(
                name: "IX_Maintenances_TenantId",
                table: "Maintenances");

            migrationBuilder.DropIndex(
                name: "IX_MaintenancePayments_TenantId",
                table: "MaintenancePayments");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceCenters_TenantId",
                table: "MaintenanceCenters");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceCenterPhones_TenantId",
                table: "MaintenanceCenterPhones");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_TenantId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_ExpenseCategories_TenantId",
                table: "ExpenseCategories");

            migrationBuilder.DropIndex(
                name: "IX_Customers_TenantId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Cars_TenantId",
                table: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_CarImages_TenantId",
                table: "CarImages");

            migrationBuilder.DropIndex(
                name: "IX_CarFeatures_TenantId",
                table: "CarFeatures");

            migrationBuilder.DropColumn(
                name: "IsPlatformAdmin",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ShareViews");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ShareContacts");

            migrationBuilder.DropColumn(
                name: "PriceSource",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "PublicShares");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "MaintenancePayments");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "MaintenanceCenters");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "MaintenanceCenterPhones");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ExpenseCategories");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "DiscountEndAt",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "DiscountStartAt",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "DiscountedPrice",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "HidePrice",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "IsDiscountActive",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "ShowContactCta",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "CarImages");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "CarFeatures");
        }
    }
}
