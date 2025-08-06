using System.ComponentModel.DataAnnotations;

namespace EmpManage.Models
{
    public class Menu
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Route { get; set; }
        public string Section { get; set; }
        public string Icon { get; set; }
        public int InOrder { get; set; }
        public int IsActive { get; set; } = 1;
        public int IsDelete { get; set; } = 0;

        public ICollection<RoleMenuPermission>? RoleMenuPermissions { get; set; }
    }
}