using EmpManage.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace EmpManage.Data
{
    public class DbSeeder
    {
        public static void SeedAdminUser(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Apply migrations
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

            // Check if Admin user exists
            var admin = context.Employees.FirstOrDefault(e => e.Email == "admin1@gmail.com");

            if (admin == null)
            {
                admin = new Employee
                {
                    Name = "SuperAdmin",
                    Email = "admin1@gmail.com",
                    PasswordHash = Convert.FromBase64String("VffjapRV/CrbjNEA3bCHZoYGnR4sBg7FG9K4tBoFiAK+ymL9b8KXb82lLV3rLqb0BXKE/NGRK15emlFEp/GXLg=="),
                    PasswordSalt = Convert.FromBase64String("sZneoUw1KBAVA4wwi5WwZ11pT3m2Yvg4xg51++fOsFPliPEp6t5eAd739+XUOeUhNtqm6/u/oj5bAq7e/lC5ciexUatoh1oS3yiJGoCRStuqQPIhPvGEdz6mzcm1nlltdPNGCNcMJz3CBfN8NhC2XlNbIrFkSXGFoEKdNY6KDCo=")
                };

                context.Employees.Add(admin);
                context.SaveChanges();
            }

            // Assign Admin role to admin user
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
