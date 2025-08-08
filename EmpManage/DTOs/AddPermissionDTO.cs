using System.Text.Json.Serialization;

namespace EmpManage.DTOs
{
    public class AddPermissionDTO
    {
        public int RoleId { get; set; }
        [JsonIgnore]
        public string? RoleName { get; set; }
        public int MenuId { get; set; }
        [JsonIgnore]
        public string? MenuName { get; set; }
        public List<int> PermissionIds { get; set; } = new();
        [JsonIgnore]
        public List<string?> PermissionNames { get; set; } = new(); 
    }
}
