using EmpManage.Data;
using EmpManage.DTOs;
using EmpManage.Helper;
using EmpManage.Interfaces;
using EmpManage.Models;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace EmpManage.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthRepository(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<bool> UserExists(string email)
        {
            return await _context.Employees.AnyAsync(u => u.Email == email);
        }

        public async Task<string?> LoginAsync(LoginDTO logindto)
        {
            var user = await _context.Employees.FirstOrDefaultAsync(e => e.Email == logindto.Email);
            if(user == null) return null;

            var hashedInput = HashPassword(logindto.Password);
            if(user.Password != hashedInput)
                return null;
            var roles = await _context.EmpRoles
                .Where(er => er.EmployeeId == user.Id)
                .Select(er => er.Role!.Name)
                .ToListAsync();

            var token = CreateJWTToken(user, roles);
            return token;
        }

        public async Task<Employee?> RegisterAsync(RegisterDTO dto, ClaimsPrincipal user)
        {
            var formattedName = char.ToUpper(dto.Name[0]) + dto.Name.Substring(1).ToLower();
            var email = dto.Email.Trim().ToLower();

            var existingUser = await _context.Employees.FirstOrDefaultAsync(e => e.Email == email);
            if (existingUser != null)
                return null;

            var employee = new Employee
            {
                Name = formattedName,
                Email = email,
                Password = HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "Self"
            };

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            if (role != null)
            {
                employee.EmpRoles = new List<EmpRole>
        {
            new EmpRole { RoleId = role.Id }
        };
            }

            await _context.Employees.AddAsync(employee);
            return employee;
        }

        private string CreateJWTToken(Employee employee, List<string> roles)
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

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hashBytes = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }
    }
} 