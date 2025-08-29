namespace EmpManage.DTOs
{
    public class UpdateRoleDTO
    {
        public int EmployeeId { get; set; }
        public List<int> RolesToAssign { get; set; } = new();
        public List<int> RolesToRemove { get; set; } = new();
    }
}