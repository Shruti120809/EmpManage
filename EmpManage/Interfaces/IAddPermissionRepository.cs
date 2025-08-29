using EmpManage.DTOs;
using EmpManage.Models;

namespace EmpManage.Interfaces
{
    public interface IAddPermissionRepository
    {
        Task<List<PermissionDTO>> GetPermissionsAsync();
        Task AssignAsyncPermission(AddPermissionDTO addPermissionDTO);
        Task RemoveAsyncPermission(int roleId, int menuId);
        Task<List<RoleMenuPermission>> GetPermissionsByRoleAsync(int roleId);
        Task SaveChangesAsync();

    }
}
