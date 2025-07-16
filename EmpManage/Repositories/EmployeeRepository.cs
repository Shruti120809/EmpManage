using EmpManage.Data;
using EmpManage.DTOs;
using EmpManage.Helper;
using EmpManage.Interfaces;
using EmpManage.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EmpManage.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.EmpRoles).ThenInclude(er => er.Role)
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _context.Employees
                .Include(e => e.EmpRoles).ThenInclude(er => er.Role)
                .Where(e => !e.IsDeleted)
                .ToListAsync();
        }

        public async Task UpdateAsync(int id, UpdateDTO updatedto, ClaimsPrincipal user)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return;

            var trimmedName = updatedto.Name.Trim();
            var formattedName = char.ToUpper(trimmedName[0]) + trimmedName.Substring(1).ToLower();

            employee.Name = formattedName;
            employee.Email = updatedto.Email.Trim().ToLower();

            employee.UpdatedBy = UserClaimsHelper.GetCurrentUserName(user);
            employee.UpdatedAt = DateTime.UtcNow;

            _context.Employees.Update(employee);
        }


        public async Task DeleteAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null && !employee.IsDeleted)
            {
                employee.IsDeleted = true;
                employee.UpdatedAt = DateTime.UtcNow;
                employee.UpdatedBy = "System";
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Employee?> GetByIdAdminAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.EmpRoles).ThenInclude(er => er.Role)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task UpdateByIdAdminAsync(int id, UpdateDTO updatedto)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                employee.Name = updatedto.Name;
                employee.Email = updatedto.Email;
                employee.UpdatedAt = DateTime.UtcNow;
                employee.UpdatedBy = "Admin";
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteByIdAdminAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                employee.IsDeleted = true;
                employee.UpdatedAt = DateTime.UtcNow;
                employee.UpdatedBy = "Admin";
                await _context.SaveChangesAsync();
            }
        }

        public async Task AssignRoleAsync(AssignRoleDTO assignrole)
        {
            foreach (var roleId in assignrole.RoleId)
            {
                var alreadyAssigned = await _context.EmpRoles
                    .AnyAsync(er => er.EmployeeId == assignrole.EmployeeId && er.RoleId == roleId);

                if (!alreadyAssigned)
                {
                    _context.EmpRoles.Add(new EmpRole
                    {
                        EmployeeId = assignrole.EmployeeId,
                        RoleId = roleId
                    });
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task RemoveRoleAsync(RemoveRoleDTO dto)
        {
            var roleAssignments = _context.EmpRoles
                .Where(er => er.EmployeeId == dto.EmployeeId && dto.RoleIds.Contains(er.RoleId));

            _context.EmpRoles.RemoveRange(roleAssignments);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Role>> GetRolesByIdsAsync(List<int> ids)
        {
            return await _context.Roles.Where(r => ids.Contains(r.Id)).ToListAsync();
        }

        public async Task<Role?> GetRoleByIdAsync(int id)
        {
            return await _context.Roles.FindAsync(id);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
