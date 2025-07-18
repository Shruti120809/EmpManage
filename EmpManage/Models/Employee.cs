﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmpManage.Models
{
    public class Employee : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password {  get; set; } = string.Empty;
        public ICollection<EmpRole>? EmpRoles { get; set; }
    }
}
