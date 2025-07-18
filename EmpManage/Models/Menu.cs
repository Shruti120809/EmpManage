using System.ComponentModel.DataAnnotations;

namespace EmpManage.Models
{
    public class Menu
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Route { get; set; }
        public string Section { get; set; }
        public string Icon { get; set; }
        public int Order { get; set; }

        public ICollection<RoleMenuPermission>? RoleMenuPermissions { get; set; }
    }
}
