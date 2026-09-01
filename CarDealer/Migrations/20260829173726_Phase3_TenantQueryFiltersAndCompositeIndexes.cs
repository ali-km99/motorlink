using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarDealer.API.Migrations
{
    /// <inheritdoc />
    public partial class Phase3_TenantQueryFiltersAndCompositeIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MaintenanceCenters_Name",
                table: "MaintenanceCenters");

            migrationBuilder.DropIndex(
                name: "IX_ExpenseCategories_Name",
                table: "ExpenseCategories");

            migrationBuilder.DropIndex(
                name: "IX_Cars_VinNumber",
                table: "Cars");

            migrationBuilder.CreateTable(
                name: "MarketplaceUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RefreshTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketplaceUsers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceCenters_TenantId_Name",
                table: "MaintenanceCenters",
                columns: new[] { "TenantId", "Name" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseCategories_TenantId_Name",
                table: "ExpenseCategories",
                columns: new[] { "TenantId", "Name" },
                unique: true,
                filter: "[TenantId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_TenantId_VinNumber",
                table: "Cars",
                columns: new[] { "TenantId", "VinNumber" },
                unique: true,
                filter: "[VinNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceUsers_Email",
                table: "MarketplaceUsers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MarketplaceUsers_Username",
                table: "MarketplaceUsers",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarketplaceUsers");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceCenters_TenantId_Name",
                table: "MaintenanceCenters");

            migrationBuilder.DropIndex(
                name: "IX_ExpenseCategories_TenantId_Name",
                table: "ExpenseCategories");

            migrationBuilder.DropIndex(
                name: "IX_Cars_TenantId_VinNumber",
                table: "Cars");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceCenters_Name",
                table: "MaintenanceCenters",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseCategories_Name",
                table: "ExpenseCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cars_VinNumber",
                table: "Cars",
                column: "VinNumber",
                unique: true,
                filter: "[VinNumber] IS NOT NULL");
        }
    }
}
