using System.ComponentModel.DataAnnotations.Schema;

namespace EmpManage.Models
{
    public class EmpRole
    {
        public int EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public Employee? Employee { get; set; }

        public int RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public Role? Role { get; set; }
    }
}
