using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmpManage.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public byte[]? PasswordHash { get; set; }

        [Required]
        public byte[]? PasswordSalt { get; set; }

        // ✅ Removed RoleId - managed through EmpRoles
        public ICollection<EmpRole>? EmpRoles { get; set; }
    }
}
