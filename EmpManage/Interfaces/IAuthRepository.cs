using EmpManage.DTOs;
using EmpManage.Models;

namespace EmpManage.Interfaces
{
    public interface IAuthRepository
    {
        Task<bool> UserExists(string email);
        Task<Employee> RegisterAsync(RegisterDTO registerdto);
        Task<string?> LoginAsync(LoginDTO logindto);
    }
}
