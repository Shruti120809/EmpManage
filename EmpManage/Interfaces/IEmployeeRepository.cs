using EmpManage.DTOs;
using EmpManage.Models;

namespace EmpManage.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<Employee> GetByIdAsync(int id);
        Task UpdateAsync (int id, UpdateDTO updatedto);
        Task DeleteAsync (int id);

        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee> GetByIdAdminAsync(int id);
        Task UpdateByIdAdminAsync(int id,UpdateDTO updateDto);
        Task DeleteByIdAdminAsync(int id);
        Task AssignRoleAsync(AssignRoleDTO assignrole);
        Task RemoveRoleAsync(RemoveRoleDTO dto);


        Task<bool> SaveChangesAsync();
    }
}
