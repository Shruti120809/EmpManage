using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmpManage.Migrations
{
    public partial class DropIdFromRoleMenuPermission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Replace with actual PK name from SQL result
            migrationBuilder.DropPrimaryKey(
                name: "PK__RoleMenu__3214EC07179EBA09",  // <-- paste actual name here
                table: "RoleMenuPermission");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "RoleMenuPermission");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoleMenuPermission", // or any name you want
                table: "RoleMenuPermission",
                columns: new[] { "RoleId", "MenuId", "PermissionId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop composite primary key
            migrationBuilder.DropPrimaryKey(
                name: "PK_RoleMenuPermission",
                table: "RoleMenuPermission");

            // Add Id column back
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "RoleMenuPermission",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            // Add primary key back using Id
            migrationBuilder.AddPrimaryKey(
                name: "PK_RoleMenuPermission",
                table: "RoleMenuPermission",
                column: "Id");
        }
    }
}
