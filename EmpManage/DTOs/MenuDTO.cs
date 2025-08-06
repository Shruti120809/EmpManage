using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EmpManage.DTOs
{
    public class MenuDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Route { get; set; }
        public string? Section { get; set; }
        public string? Icon { get; set; }
        public int InOrder { get; set; }
        [JsonIgnore]
        public int? IsActive { get; set; } = 1;
        [JsonIgnore]
        public int? IsDelete { get; set; }
    }
}