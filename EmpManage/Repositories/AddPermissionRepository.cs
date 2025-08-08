using AutoMapper;
using EmpManage.Data;
using EmpManage.DTOs;
using EmpManage.Interfaces;
using EmpManage.Models;
using Microsoft.EntityFrameworkCore;

namespace EmpManage.Repositories
{
    public class AddPermissionRepository : IAddPermissionRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public AddPermissionRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<RoleMenuPermission>> GetPermissionsByRoleAsync(int roleId)
        {
            return await _context.RoleMenuPermission
                .Where(x => x.RoleId == roleId)
                .ToListAsync();
        }

        public async Task AssignAsyncPermission(AddPermissionDTO addPermissionDTO)
        {
            var existing = await _context.RoleMenuPermission
                .Where(rmp => rmp.RoleId == addPermissionDTO.RoleId && rmp.MenuId == addPermissionDTO.MenuId)
                .ToListAsync();

            _context.RoleMenuPermission.RemoveRange(existing);
            var role = await _context.Roles.FindAsync(addPermissionDTO.RoleId);
            var menu = await _context.Menus.FindAsync(addPermissionDTO.MenuId);

            if (role == null || menu == null)
                return;

            foreach (var permissionId in addPermissionDTO.PermissionIds)
            {
                var permission = await _context.Permissions.FindAsync(permissionId);

                if (permission != null)
                {
                    var newEntry = new RoleMenuPermission
                    {
                        RoleId = role.Id,
                        RoleName = role.Name,
                        MenuId = menu.Id,
                        MenuName = menu.Name,
                        PermissionId = permission.Id,
                        PermissionNames = permission.Name
                    };

                    await _context.RoleMenuPermission.AddAsync(newEntry);
                }
            }
        }


        public async Task RemoveAsyncPermission(int roleId, int menuId)
        {
            var existingPermissions = await _context.RoleMenuPermission
                .Where(rmp => rmp.RoleId == roleId && rmp.MenuId == menuId)
                .ToListAsync();

            if (existingPermissions.Any())
            {
                _context.RoleMenuPermission.RemoveRange(existingPermissions);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
