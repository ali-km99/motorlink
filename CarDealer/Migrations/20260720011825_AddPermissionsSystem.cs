using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CarDealer.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPermissionsSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPermissions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Category", "Code", "Name" },
                values: new object[,]
                {
                    { 1, "Cars", "Cars.View", "عرض السيارات" },
                    { 2, "Cars", "Cars.Create", "إضافة سيارة" },
                    { 3, "Cars", "Cars.Update", "تعديل سيارة" },
                    { 4, "Cars", "Cars.Delete", "حذف سيارة" },
                    { 5, "Cars", "Cars.UploadImages", "رفع صور السيارة" },
                    { 6, "Maintenance", "Maintenance.View", "عرض الصيانات" },
                    { 7, "Maintenance", "Maintenance.Create", "إضافة صيانة" },
                    { 8, "Maintenance", "Maintenance.Update", "تعديل صيانة" },
                    { 9, "Maintenance", "Maintenance.Delete", "حذف صيانة" },
                    { 10, "Customers", "Customers.View", "عرض العملاء" },
                    { 11, "Customers", "Customers.Create", "إضافة عميل" },
                    { 12, "Sales", "Sales.View", "عرض المبيعات" },
                    { 13, "Sales", "Sales.Create", "تسجيل عملية بيع" },
                    { 14, "Transactions", "Transactions.View", "عرض المعاملات المالية" },
                    { 15, "Users", "Users.Manage", "إدارة المستخدمين والصلاحيات" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Code",
                table: "Permissions",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_PermissionId",
                table: "UserPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_UserId_PermissionId",
                table: "UserPermissions",
                columns: new[] { "UserId", "PermissionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPermissions");

            migrationBuilder.DropTable(
                name: "Permissions");
        }
    }
}
