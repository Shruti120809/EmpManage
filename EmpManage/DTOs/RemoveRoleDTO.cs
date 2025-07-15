namespace EmpManage.DTOs
{
    public class RemoveRoleDTO
    {
        public int EmployeeId { get; set; }
        public List<int> RoleIds { get; set; } = new();
    }
}
