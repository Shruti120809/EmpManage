using EmpManage.DTOs;
using EmpManage.Models;
using System.Security.Claims;

namespace EmpManage.Interfaces
{
    public interface IAuthRepository
    {
        Task<bool> UserExists(string email);
        Task<Employee> RegisterAsync(RegisterDTO registerdto, ClaimsPrincipal user);
        Task<string?> LoginAsync(LoginDTO logindto);
    }
}
