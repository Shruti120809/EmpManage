using EmpManage.Data;
using EmpManage.DTOs;
using EmpManage.Helper;
using EmpManage.Interfaces;
using EmpManage.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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

        public async Task<Employee?> GetByEmailAsync(string email)
        {
            return await _context.Employees
                .Include(e => e.EmpRoles)
                    .ThenInclude(er => er.Role)
                .FirstOrDefaultAsync(e => e.Email == email);
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.EmpRoles).ThenInclude(er => er.Role)
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }

        public async Task<PaginationDTO<EmployeeDTO>> GetAllAsync(SortingPaginationDTO dto)
        {
            var query = _context.Employees
                .Include(e => e.EmpRoles)!
                    .ThenInclude(er => er.Role)!
                        .ThenInclude(r => r.RoleMenuPermissions)!
                            .ThenInclude(rmp => rmp.Menu)
                .AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(dto.Search))
            {
                string lowerSearch = dto.Search.ToLower();
                query = query.Where(e =>
                    e.Name.ToLower().Contains(lowerSearch) ||
                    e.Email.ToLower().Contains(lowerSearch));
            }

            // Sorting
            query = dto.Sorting?.ToLower() switch
            {
                "name" => dto.IsAsc ? query.OrderBy(e => e.Name) : query.OrderByDescending(e => e.Name),
                "email" => dto.IsAsc ? query.OrderBy(e => e.Email) : query.OrderByDescending(e => e.Email),
                _ => query.OrderBy(e => e.Id)
            };

            int totalCount = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalCount / (double)dto.PageSize);

            var paginatedData = await query
                .Skip((dto.PageIndex - 1) * dto.PageSize)
                .Take(dto.PageSize)
                .ToListAsync();

            var employeeList = paginatedData.Select(e => new EmployeeDTO
            {
                Id = e.Id,
                Name = e.Name,
                Email = e.Email,
                Roles = e.EmpRoles?.Select(er => er.Role.Name).ToList() ?? new(),
                Menus = e.EmpRoles?
                    .SelectMany(er => er.Role.RoleMenuPermissions!)
                    .Where(rmp => rmp.Menu != null)
                    .Select(rmp => rmp.Menu!.Name)
                    .Distinct()
                    .ToList() ?? new()
            }).ToList();

            return new PaginationDTO<EmployeeDTO>
            {
                Items = employeeList,
                PageIndex = dto.PageIndex,
                PageSize = dto.PageSize,
                TotalPages = totalPages
            };
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
