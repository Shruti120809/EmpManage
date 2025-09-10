using EmpManage.Data;
using EmpManage.DTOs;
using EmpManage.Interfaces;
using EmpManage.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EmpManage.Interfaces
{
    public class CreateTokenService : ICreateTokenService
    {
        private readonly IConfiguration _config;

        public CreateTokenService(AppDbContext context, IConfiguration config)
        {
            _config = config;
        }

        public string CreateJWTToken(Employee employee, List<string> roles, string? mimickedBy = null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString()),
                new Claim(ClaimTypes.Name, employee.Name),
                new Claim(ClaimTypes.Email, employee.Email)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            if (!string.IsNullOrEmpty(mimickedBy))
            {
                claims.Add(new Claim("IsMimic", "true"));
                claims.Add(new Claim("MimickedBy", mimickedBy));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}