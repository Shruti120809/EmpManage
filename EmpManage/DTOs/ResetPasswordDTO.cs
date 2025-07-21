using System.ComponentModel.DataAnnotations;

namespace EmpManage.DTOs
{
    public class ResetPasswordDTO
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Otp { get; set; } = string.Empty;
        public string Password { get; set; } =string.Empty;
    }
}
