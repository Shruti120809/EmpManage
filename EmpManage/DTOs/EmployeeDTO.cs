using System.Text.Json.Serialization;

namespace EmpManage.DTOs
{
    public class EmployeeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string>? Roles { get; set; }
        public List<string>? Menus { get; set; } = new();
    }
}
