using System.ComponentModel.DataAnnotations;

namespace EmpManage.Models
{
    public class RoleMenuPermission
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public Role? Role { get; set; }

        public int PermissionId { get; set; }
        public string PermissionName { get; set; } = string.Empty ;
        public Permission? Permission { get; set; }

        public int MenuId { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public Menu? Menu { get; set; }
    }
}
