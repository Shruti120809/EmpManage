using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmpManage.Migrations
{
    /// <inheritdoc />
    public partial class MenuTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Order",
                table: "Menus",
                newName: "InOrder");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InOrder",
                table: "Menus",
                newName: "Order");
        }
    }
}
