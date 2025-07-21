using System.ComponentModel.DataAnnotations;

namespace EmpManage.DTOs
{
    public class ForgetPasswordDTO
    {
        [Required(ErrorMessage ="Enter Email")]
        [EmailAddress(ErrorMessage ="Enter valid emailid")]
        public string Email { get; set; } = string.Empty;
    }
}
