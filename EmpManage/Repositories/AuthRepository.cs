using EmpManage.Data;
using EmpManage.DTOs;
using EmpManage.Interfaces;
using EmpManage.Models;
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

            if(!VerifyPasswordHash(logindto.Password, user.PasswordHash, user.PasswordSalt)) return null;

            var roles = await _context.EmpRoles
                .Where(er => er.EmployeeId == user.Id)
                .Select(er => er.Role!.Name)
                .ToListAsync();

            var token = CreateJWTToken(user, roles);
            return token;
        }

        public async Task<Employee> RegisterAsync(RegisterDTO registerdto)
        {
            CreatePasswordHash(registerdto.Password, out byte[] passwordhash, out byte[] passwordsalt);

            var employee = new Employee
            {
                Name = registerdto.Name,
                Email = registerdto.Email,
                PasswordHash = passwordhash,
                PasswordSalt = passwordsalt,
            };

            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();

            var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            if(defaultRole != null)
            {
                await _context.EmpRoles.AddAsync(new EmpRole
                {
                    EmployeeId = employee.Id,
                    RoleId = defaultRole.Id
                });

                await _context.SaveChangesAsync();
            }

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

        private void CreatePasswordHash (string password, out byte[] hash, out byte[] salt)
        {
            using var hmac = new HMACSHA512();
            salt = hmac.Key;
            hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPasswordHash (string password, byte[] storedhash, byte[] storedSalt)
        {
            using var hmac = new HMACSHA512(storedSalt);
            var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computeHash.SequenceEqual(storedhash);
        }
    }
}
