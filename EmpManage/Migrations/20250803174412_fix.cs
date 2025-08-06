using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmpManage.Migrations
{
    /// <inheritdoc />
    public partial class fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MenuId",
                table: "EmployeeDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Permissions",
                table: "EmployeeDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MenuId",
                table: "EmployeeDetails");

            migrationBuilder.DropColumn(
                name: "Permissions",
                table: "EmployeeDetails");
        }
    }
}
