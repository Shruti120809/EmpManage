namespace EmpManage.DTOs
{
    public class MimicResponseDTO
    {
        public MimicUserInfoDTO User { get; set; } = null!;
        public string Token { get; set; } = string.Empty;

    }

    public class MimicUserInfoDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<RoleDTO> Roles { get; set; }
    }
}
