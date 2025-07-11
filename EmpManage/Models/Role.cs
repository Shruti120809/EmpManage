using System.ComponentModel.DataAnnotations;

namespace EmpManage.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        public ICollection<EmpRole>? EmpRoles { get; set; } //Navigation Property
    }
}
