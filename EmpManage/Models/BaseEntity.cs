namespace EmpManage.Models
{
    public class BaseEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime UpdatedAt {  get; set; }
        public string? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
