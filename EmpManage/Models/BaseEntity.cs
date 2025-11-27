namespace EmpManage.Models
{
    public class BaseEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt {  get; set; }
        public string? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; } = false;

        public Guid? ResetToken { get; set; }
        public DateTime? ResetTokenGeneratedAt { get; set;}

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpireAt { get; set; }
    }
}