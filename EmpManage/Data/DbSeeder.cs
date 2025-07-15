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
            Console.WriteLine("🔄 Seeding process started...");

            try
            {
                using var scope = app.Services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Apply migrations
                Console.WriteLine("📦 Applying migrations...");
                context.Database.Migrate();

                // ✅ Seed Roles
                if (!context.Roles.Any())
                {
                    Console.WriteLine("✅ Seeding roles...");
                    context.Roles.AddRange(
                        new Role { Name = "Admin" },
                        new Role { Name = "User" },
                        new Role { Name = "Hr" },
                        new Role { Name = "Manager" },
                        new Role { Name = "Intern" }
                    );
                    context.SaveChanges();
                }
                else
                {
                    Console.WriteLine("ℹ️ Roles already exist.");
                }

                // ✅ Check if Admin user exists
                var admin = context.Employees.FirstOrDefault(e => e.Email == "admin1@gmail.com");

                if (admin == null)
                {
                    Console.WriteLine("✅ Creating SuperAdmin user...");
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

                    Console.WriteLine("✅ SuperAdmin created successfully.");
                }
                else
                {
                    Console.WriteLine("ℹ️ SuperAdmin already exists.");
                }

                // ✅ Re-fetch admin to ensure Id is available
                admin = context.Employees.First(e => e.Email == "admin1@gmail.com");

                // ✅ Assign Admin role if not already assigned
                var adminRole = context.Roles.FirstOrDefault(r => r.Name == "Admin");

                if (adminRole != null && !context.EmpRoles.Any(er => er.EmployeeId == admin.Id && er.RoleId == adminRole.Id))
                {
                    Console.WriteLine("✅ Assigning Admin role to SuperAdmin...");
                    context.EmpRoles.Add(new EmpRole
                    {
                        EmployeeId = admin.Id,
                        RoleId = adminRole.Id
                    });

                    context.SaveChanges();

                    Console.WriteLine("✅ Admin role assigned successfully.");
                }
                else
                {
                    Console.WriteLine("ℹ️ Admin role already assigned or role not found.");
                }

                Console.WriteLine("🎉 Seeding process completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error during seeding: {ex.Message}");
            }
        }
    }
}
