using EmpManage.DTOs;
using EmpManage.Models;
using System.Security.Claims;

namespace EmpManage.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<EmployeeDTO?> GetByIdAsync(int id);
        Task<PaginationDTO<EmployeeDTO>> GetAllAsync(SortingPaginationDTO dto);
        Task<bool> UpdateAsync(int id, UpdateDTO updatedto, ClaimsPrincipal user);
        Task<DeleteDTO> DeleteAsync(int id);
        Task<Employee?> GetByIdAdminAsync(int id);  
        Task<UpdateDTO> UpdateByIdAdminAsync(int id, UpdateDTO updatedto);

        Task<AssignRoleDTO> AssignRoleAsync(AssignRoleDTO assignrole);
        Task<RemoveRoleDTO> RemoveRoleAsync(RemoveRoleDTO dto);

        Task<List<Role>> GetRolesByIdsAsync(List<int> ids);
        Task<Role?> GetRoleByIdAsync(int id);

        Task<bool> SaveChangesAsync();
    }
}
