using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarDealer.API.Migrations
{
    /// <inheritdoc />
    public partial class AddMaintenanceCenterPhonesAndDashboardDebts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaintenanceCenterPhones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaintenanceCenterId = table.Column<int>(type: "int", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceCenterPhones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MaintenanceCenterPhones_MaintenanceCenters_MaintenanceCenterId",
                        column: x => x.MaintenanceCenterId,
                        principalTable: "MaintenanceCenters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceCenterPhones_MaintenanceCenterId",
                table: "MaintenanceCenterPhones",
                column: "MaintenanceCenterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaintenanceCenterPhones");
        }
    }
}
