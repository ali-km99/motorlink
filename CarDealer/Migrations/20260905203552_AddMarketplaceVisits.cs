using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CarDealer.API.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketplaceVisits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantSubscriptions_SubscriptionPlans_SubscriptionPlanId",
                table: "TenantSubscriptions");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SubscriptionPlans",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "SubscriptionPlans",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "MarketplaceVisits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MarketplaceUserId = table.Column<int>(type: "int", nullable: true),
                    VisitedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketplaceVisits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MarketplaceVisits_MarketplaceUsers_MarketplaceUserId",
                        column: x => x.MarketplaceUserId,
                        principalTable: "MarketplaceUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "SubscriptionPlans",
                columns: new[] { "Id", "AllowExpensesModule", "AllowMaintenanceDebtReports", "AllowPublicSharing", "Code", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, false, false, false, "Basic", true, "الأساسية" },
                    { 2, false, true, true, "Professional", true, "الاحترافية" },
                    { 3, true, true, true, "Business", true, "الأعمال" }
                });

            migrationBuilder.InsertData(
                table: "TenantSubscriptions",
                columns: new[] { "Id", "EndedAt", "IsActive", "StartedAt", "SubscriptionPlanId", "TenantId" },
                values: new object[] { 1, null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_TenantSubscriptions_TenantId_IsActive",
                table: "TenantSubscriptions",
                columns: new[] { "TenantId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPlans_Code",
                table: "SubscriptionPlans",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceVisits_MarketplaceUserId",
                table: "MarketplaceVisits",
                column: "MarketplaceUserId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceVisits_VisitedAt",
                table: "MarketplaceVisits",
                column: "VisitedAt");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantSubscriptions_SubscriptionPlans_SubscriptionPlanId",
                table: "TenantSubscriptions",
                column: "SubscriptionPlanId",
                principalTable: "SubscriptionPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantSubscriptions_SubscriptionPlans_SubscriptionPlanId",
                table: "TenantSubscriptions");

            migrationBuilder.DropTable(
                name: "MarketplaceVisits");

            migrationBuilder.DropIndex(
                name: "IX_TenantSubscriptions_TenantId_IsActive",
                table: "TenantSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_SubscriptionPlans_Code",
                table: "SubscriptionPlans");

            migrationBuilder.DeleteData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TenantSubscriptions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SubscriptionPlans",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SubscriptionPlans",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "SubscriptionPlans",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantSubscriptions_SubscriptionPlans_SubscriptionPlanId",
                table: "TenantSubscriptions",
                column: "SubscriptionPlanId",
                principalTable: "SubscriptionPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
