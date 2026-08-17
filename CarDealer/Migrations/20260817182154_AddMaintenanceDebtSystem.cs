using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarDealer.API.Migrations
{
    /// <inheritdoc />
    public partial class AddMaintenanceDebtSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_Cars_CarId",
                table: "Maintenances");

            migrationBuilder.AddColumn<int>(
                name: "MaintenanceCenterId",
                table: "Maintenances",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MaintenanceCenters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceCenters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaintenancePayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaintenanceId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenancePayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenancePayments_Maintenances_MaintenanceId",
                        column: x => x.MaintenanceId,
                        principalTable: "Maintenances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Maintenances_MaintenanceCenterId",
                table: "Maintenances",
                column: "MaintenanceCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Maintenances_MaintenanceCenterId_CreatedAt",
                table: "Maintenances",
                columns: new[] { "MaintenanceCenterId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceCenters_Name",
                table: "MaintenanceCenters",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePayments_MaintenanceId",
                table: "MaintenancePayments",
                column: "MaintenanceId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenancePayments_PaymentDate",
                table: "MaintenancePayments",
                column: "PaymentDate");

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_Cars_CarId",
                table: "Maintenances",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_MaintenanceCenters_MaintenanceCenterId",
                table: "Maintenances",
                column: "MaintenanceCenterId",
                principalTable: "MaintenanceCenters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_Cars_CarId",
                table: "Maintenances");

            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_MaintenanceCenters_MaintenanceCenterId",
                table: "Maintenances");

            migrationBuilder.DropTable(
                name: "MaintenanceCenters");

            migrationBuilder.DropTable(
                name: "MaintenancePayments");

            migrationBuilder.DropIndex(
                name: "IX_Maintenances_MaintenanceCenterId",
                table: "Maintenances");

            migrationBuilder.DropIndex(
                name: "IX_Maintenances_MaintenanceCenterId_CreatedAt",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "MaintenanceCenterId",
                table: "Maintenances");

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_Cars_CarId",
                table: "Maintenances",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
