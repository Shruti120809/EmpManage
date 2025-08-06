using System.ComponentModel.DataAnnotations;

namespace EmpManage.DTOs
{
    public class UpdateDTO
    {
        [Required(ErrorMessage = "Name is required.")]
        [MinLength(2, ErrorMessage = "Name must be at least 2 characters.")]
        [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "Name cannot be blank or just spaces.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

    }
}
