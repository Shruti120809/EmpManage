using System.ComponentModel.DataAnnotations;

namespace EmpManage.DTOs
{
    public class ResetPasswordDTO
    {
        public string Token { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }
    }

}
