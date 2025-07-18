using System.ComponentModel.DataAnnotations;

namespace EmpManage.Models
{
    public class Permission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<RoleMenuPermission>? RoleMenuPermission { get; set; }
    }
}
