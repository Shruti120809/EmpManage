using EmpManage.DTOs;

public class EmployeeDetailsDTO
{
    public int Id { get; set; }             // ✅ For CS0117
    public string Name { get; set; }
    public string Email { get; set; }

    public List<string> Roles { get; set; } = new();
    public List<MenuPermissionDTO> Menus { get; set; } = new(); // ✅ For CS0117
}
