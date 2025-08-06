using EmpManage.DTOs;
using EmpManage.Models;

namespace EmpManage.Interfaces
{
    public interface IMenuRepository
    {
        Task<bool> HasMenuAsync(int id);
        Task<Menu> CreateMenuAsync(MenuDTO dto);
        Task<List<Menu>> GetMenuAsync();
        Task<Menu> GetMenuByIdAsync(int id);
        Task<Menu> UpdateMenuAsync(MenuDTO dto, int id);
        Task<bool> DeleteMenuAsync(int id);
    }
}