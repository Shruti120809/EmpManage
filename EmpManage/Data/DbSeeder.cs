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

            // Apply migrations
            context.Database.Migrate();

            bool dataChanged = false;

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
                dataChanged = true;
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
                dataChanged = true;
            }

            // Seed Menus
            //if (!context.Menus.Any())
            //{
            //    context.Menus.AddRange(
            //        );
            //    dataChanged = true;
            //}

            // Seed RoleMenuPermission for Admin
            //if (!context.RoleMenuPermission.Any())
            //{
            //    context.RoleMenuPermission.AddRange(
            //          );
            //    dataChanged = true;
            //}

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
                context.SaveChanges(); // <-- MUST save here so admin.Id is generated
            }

            // Assign Admin Role
            var adminRole = context.Roles.FirstOrDefault(r => r.Name == "Admin");
            if (adminRole != null)
            {
                var existingEmpRole = context.EmpRoles
                    .FirstOrDefault(er => er.EmployeeId == admin.Id && er.RoleId == adminRole.Id);

                if (existingEmpRole == null)
                {
                    context.EmpRoles.Add(new EmpRole
                    {
                        EmployeeId = admin.Id,   // <-- Now has a valid Id
                        RoleId = adminRole.Id
                    });
                    context.SaveChanges(); // <-- Save EmpRole
                }
            }


            // Save all changes in a single transaction
            if (dataChanged)
            {
                context.SaveChanges();
            }
        }
    }
}
