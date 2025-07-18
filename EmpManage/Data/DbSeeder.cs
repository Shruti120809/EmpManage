using EmpManage.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace EmpManage.Data
{
    public class DbSeeder
    {
        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hashBytes = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }

        public static void SeedAdminUser(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Apply migration
            context.Database.Migrate();

            // Seed Roles
            if (!context.Roles.Any())
            {
                context.Roles.AddRange(
                    new Role { Name = "Admin" },
                    new Role { Name = "User" },
                    new Role { Name = "Hr" },
                    new Role { Name = "Manager" },
                    new Role { Name = "Intern" }
                );
                context.SaveChanges();
            }

            // Seed Permissions
            if (!context.Permissions.Any())
            {
                context.Permissions.AddRange(
                    new Permission { Name = "Create" },
                    new Permission { Name = "Read" },
                    new Permission { Name = "Update" },
                    new Permission { Name = "Delete" }
                );
                context.SaveChanges();
            }

            // Seed Menus
            if (!context.Menus.Any())
            {
                context.Menus.AddRange(
                    new Menu { Name = "Assign Role", Route = "/assign-role", Section = "Admin", Icon = "🛡️", Order = 1 },
                    new Menu { Name = "Revoke Role", Route = "/revoke-role", Section = "Admin", Icon = "❌", Order = 2 },
                    new Menu { Name = "Manage Roles", Route = "/manage-role", Section = "Admin", Icon = "🏷️", Order = 3 },
                    new Menu { Name = "Add Permission", Route = "/add-permission", Section = "Permissions", Icon = "➕", Order = 1 },
                    new Menu { Name = "View Permissions", Route = "/view-permission", Section = "Permissions", Icon = "📜", Order = 2 },
                    new Menu { Name = "Add Employee", Route = "/add-employee", Section = "Employees", Icon = "➕", Order = 1 },
                    new Menu { Name = "View Employees", Route = "/view-employees", Section = "Employees", Icon = "👀", Order = 2 },
                    new Menu { Name = "Edit Employee", Route = "/edit-employee", Section = "Employees", Icon = "✏️", Order = 3 },
                    new Menu { Name = "Delete Employee", Route = "/delete-employee", Section = "Employees", Icon = "❌", Order = 4 },
                    new Menu { Name = "View Logs", Route = "/view-logs", Section = "Reports & Logs", Icon = "📂", Order = 1 },
                    new Menu { Name = "Access Reports", Route = "/access-reports", Section = "Reports & Logs", Icon = "📈", Order = 2 },
                    new Menu { Name = "Profile", Route = "/profile", Section = "Settings", Icon = "👤", Order = 1 },
                    new Menu { Name = "Logout", Route = "/logout", Section = "Settings", Icon = "🚪", Order = 2 }
                );
                context.SaveChanges();
            }

            // Seed RoleMenuPermission for Admin
            if (!context.RoleMenuPermission.Any())
            {
                context.RoleMenuPermission.AddRange(
                    new RoleMenuPermission { RoleId = 1, RoleName = "Admin", PermissionId = 2, PermissionName = "Read" ,MenuId = 1, MenuName = "Assign Role"},
                    new RoleMenuPermission { RoleId = 1, RoleName = "Admin", PermissionId = 4, PermissionName = "Delete", MenuId = 2, MenuName = "Revoke Role" },
                    new RoleMenuPermission { RoleId = 1, RoleName = "Admin", PermissionId = 3, PermissionName = "Update", MenuId = 3, MenuName = "Manage Roles" },

                    new RoleMenuPermission { RoleId = 1, RoleName = "Admin", PermissionId = 1, PermissionName = "Create", MenuId = 4, MenuName = "Add Permission" },
                    new RoleMenuPermission { RoleId = 1, RoleName = "Admin", PermissionId = 2, PermissionName = "Read", MenuId = 5, MenuName = "View Permissions" },

                    new RoleMenuPermission { RoleId = 1, RoleName = "Admin", PermissionId = 1, PermissionName = "Create", MenuId = 6, MenuName = "Add Employee" },
                    new RoleMenuPermission { RoleId = 1, RoleName = "Admin", PermissionId = 2, PermissionName = "Read", MenuId = 7, MenuName = "View Employees" },
                    new RoleMenuPermission { RoleId = 1, RoleName = "Admin", PermissionId = 3, PermissionName = "Update", MenuId = 8, MenuName = "Edit Employee" },
                    new RoleMenuPermission { RoleId = 1, RoleName = "Admin", PermissionId = 4, PermissionName = "Delete", MenuId = 9, MenuName = "Delete Employee" },

                    new RoleMenuPermission { RoleId = 1, RoleName = "Admin", PermissionId = 2, PermissionName = "Read", MenuId = 10, MenuName = "View Logs" },
                    new RoleMenuPermission { RoleId = 1, RoleName = "Admin", PermissionId = 2, PermissionName = "Read", MenuId = 11, MenuName = "Access Reports" },

                    new RoleMenuPermission { RoleId = 1, RoleName = "Admin", PermissionId = 2, PermissionName = "Read", MenuId = 12, MenuName = "Profile" },
                    new RoleMenuPermission { RoleId = 1, RoleName = "Admin", PermissionId = 2, PermissionName = "Read", MenuId = 13, MenuName = "Logout" }
                );
                context.SaveChanges();
            }

            // Seed Admin User
            var admin = context.Employees.FirstOrDefault(e => e.Email == "admin1@gmail.com");
            if (admin == null)
            {
                admin = new Employee
                {
                    Name = "SuperAdmin",
                    Email = "admin1@gmail.com",
                    Password = HashPassword("Admin@1"),
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };

                context.Employees.Add(admin);
                context.SaveChanges();
                Console.WriteLine("New Admin ID: " + admin.Id);
            }

            // Assign Admin Role
            var adminRole = context.Roles.FirstOrDefault(r => r.Name == "Admin");
            if (adminRole != null && !context.EmpRoles.Any(er => er.EmployeeId == admin.Id && er.RoleId == adminRole.Id))
            {
                context.EmpRoles.Add(new EmpRole
                {
                    EmployeeId = admin.Id,
                    RoleId = adminRole.Id
                });

                context.SaveChanges();
            }
        }
    }
}