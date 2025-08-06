using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmpManage.Migrations
{
    /// <inheritdoc />
    public partial class Final : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "EmployeeDetails");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "EmployeeDetails");

            migrationBuilder.DropColumn(
                name: "MenuId",
                table: "EmployeeDetails");

            migrationBuilder.DropColumn(
                name: "PermissionName",
                table: "EmployeeDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "EmployeeDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "EmployeeDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MenuId",
                table: "EmployeeDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PermissionName",
                table: "EmployeeDetails",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
