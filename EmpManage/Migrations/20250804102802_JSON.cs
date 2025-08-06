using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmpManage.Migrations
{
    /// <inheritdoc />
    public partial class JSON : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MenuName",
                table: "EmployeeDetails",
                newName: "MenuJson");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MenuJson",
                table: "EmployeeDetails",
                newName: "MenuName");
        }
    }
}
