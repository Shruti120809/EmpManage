public class AssignRoleDTO
{
    public int EmployeeId { get; set; }
    public List<int> RoleIds { get; set; } = new();
}