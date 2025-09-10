using EmpManage.DTOs;
using EmpManage.Models;
using System.Security.Claims;

namespace EmpManage.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetEntityByIdAsync(int id);
        Task<EmployeeDTO?> GetByIdAsync(int id);
        Task<PaginationDTO<EmployeeDTO>> GetAllAsync(SortingPaginationDTO dto);
        Task<bool> UpdateAsync(int id, UpdateDTO updatedto, ClaimsPrincipal user);
        Task<DeleteDTO> DeleteAsync(int id);
        Task<UpdateDTO> UpdateByIdAdminAsync(int id, UpdateDTO updatedto);
        Task<UpdateRoleDTO> UpdateRolesAsync(UpdateRoleDTO dto);
        Task<MimicResponseDTO?> MimicUserAsync(MimicUserDTO user);
        Task<List<RoleDTO>> GetAllRoleAsync();
        Task<bool> SaveChangesAsync();
    }
}