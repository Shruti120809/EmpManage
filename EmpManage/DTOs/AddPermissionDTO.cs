namespace EmpManage.DTOs
{
    public class AddPermissionDTO
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public int MenuId { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public List<int> PermissionIds { get; set; } = new();
    }
}
