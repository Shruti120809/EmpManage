namespace EmpManage.DTOs
{
    public class AddPermissionDTO
    {
        public int RoleId { get; set; }
        public int MenuId { get; set; }
        public List<int> PermissionIds { get; set; } = new();
    }
}
