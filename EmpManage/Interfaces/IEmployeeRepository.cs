using EmpManage.DTOs;
using EmpManage.Models;
using System.Security.Claims;

namespace EmpManage.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetByIdAsync(int id);
        Task<PaginationDTO<EmployeeDTO>> GetAllAsync(SortingPaginationDTO dto);
        Task UpdateAsync(int id, UpdateDTO updatedto, ClaimsPrincipal user);
        Task DeleteAsync(int id); // soft delete        
        Task<Employee?> GetByIdAdminAsync(int id);
        Task UpdateByIdAdminAsync(int id, UpdateDTO updatedto);
        Task DeleteByIdAdminAsync(int id);

        Task AssignRoleAsync(AssignRoleDTO assignrole);
        Task RemoveRoleAsync(RemoveRoleDTO dto);

        Task<List<Role>> GetRolesByIdsAsync(List<int> ids);
        Task<Role?> GetRoleByIdAsync(int id);

        Task<bool> SaveChangesAsync();
    }
}
