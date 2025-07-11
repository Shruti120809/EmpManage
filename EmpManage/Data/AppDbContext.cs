using EmpManage.Models;
using Microsoft.EntityFrameworkCore;
namespace EmpManage.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext (DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<EmpRole> EmpRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite key for many-to-many relation
            modelBuilder.Entity<EmpRole>()
                .HasKey(er => new { er.EmployeeId, er.RoleId });
        }

    }
}
