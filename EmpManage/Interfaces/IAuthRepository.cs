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
        Task<Employee> VerifyOtpAsync(string email, string otp);
        Task<bool> ResetPasswordAsync(string email, string newPassword);
    }
}
