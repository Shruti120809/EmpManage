using AutoMapper;
using EmpManage.Data;
using EmpManage.DTOs;
using EmpManage.Helper;
using EmpManage.Interfaces;
using EmpManage.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EmpManage.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly IEmailRepository _emailRepository;

        public AuthRepository(AppDbContext context, IConfiguration config, IMapper mapper, IEmailRepository emailRepository)
        {
            _context = context;
            _config = config;
            _mapper = mapper;
            _emailRepository = emailRepository;
        }

        public async Task<bool> UserExists(string email)
        {
            return await _context.Employees.AnyAsync(u => u.Email == email);
        }

        public async Task<Employee?> GetUserByEmailAsync(string email)
        {
            return await _context.Employees.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<LoginResponseDTO?> LoginAsync(LoginDTO logindto)
        {
            var user = await _context.Employees
                .Include(e => e.EmpRoles!)
                    .ThenInclude(er => er.Role)
                .FirstOrDefaultAsync(e => e.Email == logindto.Email);

            if (user == null) return null;

            var hashedInput = HashPassword(logindto.Password);
            if (user.Password != hashedInput)
                return null;

            var roleIds = user.EmpRoles!.Select(r => r.RoleId).ToList();
            var roleNames = user.EmpRoles?
                .Where(r => r.Role != null)
                .Select(r => r.Role.Name)
                .ToList() ?? new List<string>();

            var rolePermissions = await _context.RoleMenuPermission
                .Where(rmp => roleIds.Contains(rmp.RoleId))
                .ToListAsync();

            var menus = rolePermissions
                .GroupBy(rmp => new
                {
                    rmp.MenuId,
                    rmp.MenuName
                })
                .Select(group => new MenuPermissionDTO
                {
                    MenuId = group.Key.MenuId,
                    MenuName = group.Key.MenuName,
                    Permissions = group
                        .GroupBy(p => new { p.PermissionId, p.PermissionNames })
                        .Select(p => new PermissionDTO
                        {
                            Id = p.Key.PermissionId,
                            Name = p.Key.PermissionNames
                        })
                        .ToList()
                })
                .ToList();

            var token = CreateJWTToken(user, roleNames);

            var loginResponse = _mapper.Map<LoginResponseDTO>(user);
            loginResponse.Token = token;
            loginResponse.Roles = roleNames;
            loginResponse.Menus = menus;

            return loginResponse;
        }


        public async Task<EmployeeDTO?> RegisterAsync(RegisterDTO dto)
        {
            var user = ClaimsPrincipal.Current;
            var trimmedName = dto.Name.Trim();
            var formattedName = char.ToUpper(trimmedName[0]) + trimmedName.Substring(1).ToLower();
            var email = dto.Email.Trim().ToLower();

            // ✅ Check if email already exists
            var existingUser = await _context.Employees.FirstOrDefaultAsync(e => e.Email == email);
            if (existingUser != null)
                return null;

            // Prepare values
            var passwordHash = HashPassword(dto.Password);
            string createdBy = "Self";

            if (user != null)
            {
                var roles = user.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();

                createdBy = roles.Contains("Admin") ? "Admin" : "Self";
            }

            // ✅ Define parameters
            var nameParam = new SqlParameter("@Name", formattedName);
            var emailParam = new SqlParameter("@Email", email);
            var passwordParam = new SqlParameter("@Password", passwordHash);
            var createdByParam = new SqlParameter("@CreatedBy", createdBy);

            // ✅ Call SP and get new Id (fixing the "non-composable SQL" issue)
            var newId = (await _context.IdResults
                .FromSqlRaw("EXEC sp_CreateEmployee @Name, @Email, @Password, @CreatedBy",
                    nameParam, emailParam, passwordParam, createdByParam)
                .ToListAsync())
                .First().NewEmployeeId;

            // ✅ Fetch employee with relations
            var employee = await _context.Employees
                .Include(e => e.EmpRoles)!
                    .ThenInclude(er => er.Role)!
                        .ThenInclude(r => r.RoleMenuPermissions)!
                            .ThenInclude(rmp => rmp.Menu)
                .FirstOrDefaultAsync(e => e.Id == newId);

            if (employee == null) return null;

            // ✅ Assign default "User" role if not already assigned
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            if (role != null && !employee.EmpRoles.Any(er => er.RoleId == role.Id))
            {
                _context.EmpRoles.Add(new EmpRole
                {
                    EmployeeId = employee.Id,
                    RoleId = role.Id
                });

                await _context.SaveChangesAsync();
            }

            // ✅ Reload employee with fresh role/menu data
            employee = await _context.Employees
                .Include(e => e.EmpRoles)!
                    .ThenInclude(er => er.Role)!
                        .ThenInclude(r => r.RoleMenuPermissions)!
                            .ThenInclude(rmp => rmp.Menu)
                .FirstOrDefaultAsync(e => e.Id == newId);

            return _mapper.Map<EmployeeDTO>(employee);
        }



        public async Task<bool> GenerateResetPasswordAsync(Employee employee)
        {
            var user = await _context.Employees.FirstOrDefaultAsync(u => u.Email == employee.Email);
            if (user == null) return false;

            var otp = new Random().Next(100000, 999999).ToString(); // 6-digit OTP
            user.Otp = otp;
            user.OtpGeneratedAt = DateTime.UtcNow;

            await _emailRepository.SendEmailAsync(user.Email, "Your OTP Code", $"Your OTP is: {otp}");
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string?> VerifyOtpAsync(VerifyOtpDTO dto)
        {
            var user = await _context.Employees.FirstOrDefaultAsync(e =>
                e.Email == dto.Email &&
                e.Otp == dto.Otp &&
                e.OtpGeneratedAt.HasValue &&
                e.OtpGeneratedAt.Value.AddMinutes(10) > DateTime.UtcNow);

            if (user == null) return null;

            Guid resetToken = Guid.NewGuid();

            user.ResetToken = resetToken;
            user.ResetTokenGeneratedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return resetToken.ToString();
        }


        public async Task<Employee> GetUserByResetTokenAsync(Guid token)
        {
            return await _context.Employees.FirstOrDefaultAsync(e => e.ResetToken == token);
        }

        public async Task<bool> ResetPasswordAsync(Guid token, string newPassword)
        {
            var user = await _context.Employees
                .FirstOrDefaultAsync(e => e.ResetToken == token);

            if (user == null || !user.ResetTokenGeneratedAt.HasValue ||
                user.ResetTokenGeneratedAt.Value.AddMinutes(15) < DateTime.UtcNow)
                return false;

            var newHashedPassword = HashPassword(newPassword);

            if (newHashedPassword == user.Password)
                return false;

            user.Password = newHashedPassword;
            user.ResetToken = null;
            user.ResetTokenGeneratedAt = null;

            _context.Employees.Update(user);
            await _context.SaveChangesAsync();

            return true;
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