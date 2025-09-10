using EmpManage.DTOs;
using EmpManage.Models;

namespace EmpManage.Interfaces
{
    public interface ICreateTokenService
    {
        public string CreateJWTToken(Employee employee, List<string> roles, string? mimickedBy = null);
    }
}