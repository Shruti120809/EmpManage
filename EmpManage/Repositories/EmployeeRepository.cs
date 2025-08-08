using AutoMapper;
using EmpManage.Data;
using EmpManage.DTOs;
using EmpManage.Helper;
using EmpManage.Interfaces;
using EmpManage.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace EmpManage.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public EmployeeRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PaginationDTO<EmployeeDTO>> GetAllAsync(SortingPaginationDTO dto)
        {
            var parameters = new[]
            {
        new SqlParameter("@Search", dto.Search ?? (object)DBNull.Value),
        new SqlParameter("@SortBy", dto.SortBy ?? "Name"),
        new SqlParameter("@IsAscending", dto.IsAscending ? 1 : 0),
        new SqlParameter("@PageIndex", dto.PageIndex),
        new SqlParameter("@PageSize", dto.PageSize),
    };

            var rawResult = await _context.EmployeeDetails
                .FromSqlRaw("EXEC sp_GetAllEmployeesPaginated @Search, @SortBy, @IsAscending, @PageIndex, @PageSize", parameters)
                .AsNoTracking()
                .ToListAsync();

            if (rawResult.Count == 0)
            {
                return new PaginationDTO<EmployeeDTO>
                {
                    Items = new List<EmployeeDTO>(),
                    PageIndex = dto.PageIndex,
                    TotalPages = 0
                };
            }

            int totalRecords = rawResult.First().TotalRecords;

            var employeeData = rawResult.Select(e => new EmployeeDTO
            {
                Id = e.EmployeeId,
                Name = e.Name,
                Email = e.Email,
                Roles = e.RoleName?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                     .Select(r => r.Trim())
                     .Distinct()
                     .ToList() ?? new(),

                Menus = !string.IsNullOrEmpty(e.MenuJson)
                    ? JsonSerializer.Deserialize<List<MenuPermissionDTO>>(e.MenuJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<MenuPermissionDTO>()
                    : new List<MenuPermissionDTO>()
            }).ToList();

            return new PaginationDTO<EmployeeDTO>
            {
                Items = employeeData,
                PageIndex = dto.PageIndex,
                PageSize = dto.PageSize,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)dto.PageSize)
            };
        }

        public async Task<EmployeeDTO?> GetByIdAsync(int id)
        {
            var param = new SqlParameter("@EmployeeId", id);

            var result = await _context.EmployeeDetailsSam
                .FromSqlRaw("EXEC sp_GetEmployeeById @EmployeeId", param)
                .AsNoTracking()
                .ToListAsync();

            if (!result.Any())
                return null;

            var first = result.First();

            var employeeDTO = result.Select(e => new EmployeeDTO
            {
                Id = first.Id,
                Name = first.Name,
                Email = first.Email,
                Roles = first.Roles?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(r => r.Trim()).ToList() ?? new(),
                Menus = !string.IsNullOrEmpty(first.MenuJson)
                    ? JsonSerializer.Deserialize<List<MenuPermissionDTO>>(first.MenuJson ?? "[]")
                    : new List<MenuPermissionDTO>()


            }).FirstOrDefault();

            return (EmployeeDTO?)employeeDTO;
        }

        public async Task<UpdateDTO> UpdateByIdAdminAsync(int id, UpdateDTO updatedto)
        {
            var parameters = new[]
            {
                new SqlParameter("@Id", id),
                new SqlParameter("@Name", updatedto.Name),
                new SqlParameter("@Email", updatedto.Email),
                new SqlParameter("@UpdatedBy", "Admin"),
                new SqlParameter("@UpdatedAt", DateTime.UtcNow)
            };

            var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_UpdateEmployeeById @Id, @Name, @Email, @UpdatedBy, @UpdatedAt", parameters);

            return updatedto;
        }


        public async Task<bool> UpdateAsync(int id, UpdateDTO updatedto, ClaimsPrincipal user)
        {
            var parameters = new[]
            {
                new SqlParameter("@Id", id),
                new SqlParameter("@Name", updatedto.Name),
                new SqlParameter("@Email", updatedto.Email),
                new SqlParameter("@UpdatedBy", UserClaimsHelper.GetCurrentUserName(user) ?? "Unknown"),
                new SqlParameter("@UpdatedAt", DateTime.UtcNow)
            };

            var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_UpdateEmployeeById @Id, @Name, @Email, @UpdatedBy, @UpdatedAt", parameters);

            return rowsAffected > 0;
        }

        public async Task<DeleteDTO> DeleteAsync(int id)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == id);
            if (employee == null)
                return null;
            var updateat = DateTime.UtcNow;

            var parameters = new[]
            {
                new SqlParameter("@Id", id),
                new SqlParameter("@UpdatedBy", "Admin"),
                new SqlParameter("@UpdatedAt", DateTime.UtcNow)
            };

            var affectedRows = await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_DeleteEmployee @Id, @UpdatedBy, @UpdatedAt", parameters);

            if (affectedRows == 0)
                return null;

            var deletedto = _mapper.Map<DeleteDTO>(employee);
            deletedto.UpdatedBy = "Admin";
            deletedto.UpdatedAt = updateat.ToString("yyyy-MM-dd HH:mm:ss");

            return deletedto;
        }

        public async Task<AssignRoleDTO> AssignRoleAsync(AssignRoleDTO assignrole)
        {
            foreach (var roleId in assignrole.RoleId)
            {
                var parameters = new[]
                {
                    new SqlParameter("@EmpId", assignrole.EmployeeId),
                    new SqlParameter("@RoleId", roleId)
                };

                await _context.Database.ExecuteSqlRawAsync("EXEC sp_AssignRole @EmpId, @RoleId", parameters);
            }

            return assignrole;
        }

        public async Task<RemoveRoleDTO> RemoveRoleAsync(RemoveRoleDTO dto)
        {
            foreach (var roleId in dto.RoleIds)
            {
                var parameters = new[]
                {
                    new SqlParameter("@EmpId", dto.EmployeeId),
                    new SqlParameter("@RoleId", roleId)
                };

                await _context.Database.ExecuteSqlRawAsync("EXEC sp_RemoveRole @EmpId, @RoleId", parameters);
            }

            return dto;
        }

        public async Task<List<RoleDTO>> GetAllRoleAsync()
        {
            var result = await _context.Roles
                .FromSqlRaw("EXEC sp_GetAllRoles")
                .AsNoTracking()
                .ToListAsync();

            var roledto = result.Select(r => new RoleDTO
            {
                Id = r.Id,
                Name = r.Name
            }).ToList();

            return roledto;
        }


        public Task<Role?> GetRoleByIdAsync(int id) => throw new NotImplementedException();
        public Task<List<Role>> GetRolesByIdsAsync(List<int> ids) => throw new NotImplementedException();
        public Task<bool> SaveChangesAsync() => throw new NotImplementedException();
    }
}


