using EmpManage.Data;
using EmpManage.DTOs;
using EmpManage.Interfaces;
using EmpManage.Models;
using Microsoft.EntityFrameworkCore;

namespace EmpManage.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public EmployeeRepository(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        //User: Get own data
        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.EmpRoles)
                .ThenInclude(er => er.Role)
                .FirstOrDefaultAsync(e => e.Id == id);
        }
        //User: Update own data
        public async Task UpdateAsync(int Id, UpdateDTO updatedto)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == Id);
            if (employee == null) return;

            employee.Name = updatedto.Name;
            employee.Email = updatedto.Email;

            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        //User: Delete own account
        public async Task DeleteAsync(int id)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);

            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }

        //Admin: Get all employees
        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _context.Employees
                .Include(e => e.EmpRoles)
                .ThenInclude(er => er.Role)
                .ToListAsync();
        }

        //Admin: Get by ID
        public async Task<Employee> GetByIdAdminAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.EmpRoles)
                .ThenInclude(er => er.Role)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        //Admin: Update by ID
        public async Task UpdateByIdAdminAsync(int Id, UpdateDTO updatedto)
        {
            var employee = await _context.Employees.FindAsync(Id);
            if (employee == null) return;

            employee.Name = updatedto.Name;
            employee.Email = updatedto.Email;

            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        //Admin: Delete User by Id
        public async Task DeleteByIdAdminAsync(int Id)
        {
            var employee = await _context.Employees.FindAsync(Id);
            if (employee != null)
            {
                var roles = _context.EmpRoles.Where(r => r.EmployeeId == Id);
                _context.EmpRoles.RemoveRange(roles);

                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }

        //Admin: Assign role to a user
        public async Task AssignRoleAsync(AssignRoleDTO dto)
        {
            foreach (var roleId in dto.RoleIds)
            {
                var exists = await _context.EmpRoles
                    .AnyAsync(er => er.EmployeeId == dto.EmployeeId && er.RoleId == roleId);

                if (!exists)
                {
                    await _context.EmpRoles.AddAsync(new EmpRole
                    {
                        EmployeeId = dto.EmployeeId,
                        RoleId = roleId
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveRoleAsync(RemoveRoleDTO dto)
        {
            var roleAssignment = await _context.EmpRoles
                .FirstOrDefaultAsync(er => er.EmployeeId == dto.EmployeeId && er.RoleId == dto.RoleId);

            if (roleAssignment != null)
            {
                _context.EmpRoles.Remove(roleAssignment);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
