using EmpManage.DTOs;
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
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<RoleMenuPermission> RoleMenuPermission { get; set; }
        public DbSet<EmployeeDetails> EmployeeDetails { get; set; }
        public DbSet<EmployeeDetailsSam> EmployeeDetailsSam { get; set; }
        public DbSet<IdResult> IdResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // many-to-many relation
            modelBuilder.Entity<EmpRole>()
                .HasKey(er => new { er.EmployeeId, er.RoleId });

            modelBuilder.Entity<RoleMenuPermission>()
                .HasKey(rmp => new { rmp.RoleId, rmp.MenuId, rmp.PermissionId });


            //soft-delete filter
            modelBuilder.Entity<Employee>().HasQueryFilter(e => !e.IsDeleted);

            modelBuilder.Entity<Employee>().HasIndex(e => e.Email)
                .IsUnique();


            modelBuilder.Entity<EmpRole>().HasQueryFilter(er => !er.Employee.IsDeleted );

            modelBuilder.Entity<EmployeeDetailsSam>().HasNoKey().ToView(null);

            modelBuilder.Entity<EmpRole>()
            .HasIndex(er => new { er.EmployeeId, er.RoleId })
            .IsUnique();

            modelBuilder.Entity<IdResult>().HasNoKey().ToView(null);

        }

    }
}
