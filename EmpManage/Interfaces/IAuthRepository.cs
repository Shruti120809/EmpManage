using EmpManage.DTOs;
using EmpManage.Models;
using System.Security.Claims;

namespace EmpManage.Interfaces
{
    public interface IAuthRepository
    {
        Task<bool> UserExists(string email);
        Task<Employee?> GetUserByEmailAsync(string email);

        Task<Employee> RegisterAsync(RegisterDTO registerdto, ClaimsPrincipal user);
        Task<LoginResponseDTO> LoginAsync(LoginDTO logindto);

        Task<bool> GenerateResetPasswordAsync(Employee employee);
        Task<string> VerifyOtpAsync(VerifyOtpDTO dto);
        Task<bool> ResetPasswordAsync(Guid token, string newPassword);
        Task<Employee> GetUserByResetTokenAsync (Guid token);
    }
}
