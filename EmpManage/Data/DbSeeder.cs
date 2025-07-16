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

            admin = context.Employees.First(e => e.Email == "admin1@gmail.com");

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
    

