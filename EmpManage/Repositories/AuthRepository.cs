﻿using AutoMapper;
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
using System.Web.Helpers;

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
            var roleNames = user.EmpRoles!.Select(r => r.Role!.Name).ToList();

            var rolePermissions = await _context.RoleMenuPermission
                .Where(rmp => roleIds.Contains(rmp.RoleId))
                .ToListAsync();

            var menus = rolePermissions
                .GroupBy(rmp => new { rmp.MenuId, rmp.MenuName })
                .Select(group => new MenuPermissionDTO
                {
                    MenuId = group.Key.MenuId,
                    MenuName = group.Key.MenuName,
                    Permissions = group.Select(p => p.PermissionName).Distinct().ToList()
                }).ToList();

            var token = CreateJWTToken(user, roleNames);

            var loginResponse = _mapper.Map<LoginResponseDTO>(user);
            loginResponse.Token = token;
            loginResponse.Roles = roleNames;
            loginResponse.Menus = menus;

            return loginResponse;
        }

        public async Task<Employee?> RegisterAsync(RegisterDTO dto, ClaimsPrincipal user)
        {
            var formattedName = char.ToUpper(dto.Name[0]) + dto.Name.Substring(1).ToLower();
            var email = dto.Email.Trim().ToLower();

            var existingUser = await _context.Employees.FirstOrDefaultAsync(e => e.Email == email);
            if (existingUser != null)
                return null;

            var trimmedName = dto.Name.Trim();

            var employee = _mapper.Map<Employee>(dto);

            employee.Name = char.ToUpper(trimmedName[0]) + trimmedName.Substring(1).ToLower();
            employee.Password = HashPassword(dto.Password);
            employee.CreatedAt = DateTime.UtcNow;
            employee.CreatedBy = "Self";

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

        public async Task<bool> GenerateResetPasswordAsync(Employee employee )
        {
            var user = await _context.Employees.FirstOrDefaultAsync(u => u.Email == employee.Email);
            if (user == null) return false;

            var otp = new Random().Next(100000, 999999).ToString();
            user.Otp = otp;
            user.OtpGeneratedAt = DateTime.UtcNow;

            await _emailRepository.SendEmailAsync(user.Email, "Your OTP Code", $"Your OTP is: {otp}");


            await _context.SaveChangesAsync(); 

            return true;

        }

        public async Task<bool> ResetPasswordAsync(string email,string otp, string newPassword)
        {
            var user = await _context.Employees.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || user.Otp != otp) return false;

            if (user.OtpGeneratedAt == null || DateTime.UtcNow > user.OtpGeneratedAt.Value.AddMinutes(10))
                return false; // OTP expired

            user.Password = HashPassword(newPassword); 
            user.Otp = null;
            user.OtpGeneratedAt = null;

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