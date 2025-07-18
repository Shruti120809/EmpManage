namespace EmpManage.DTOs
{
    public class MenuPermissionDTO
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new();
    }
}
