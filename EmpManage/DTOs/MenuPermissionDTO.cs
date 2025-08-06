namespace EmpManage.DTOs
{
    public class MenuPermissionDTO
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public List<PermissionDTO> Permissions { get; set; } = new();
    }

    public class PermissionDTO
    {
        public string Name { get; set; } = string.Empty;
    }
}
