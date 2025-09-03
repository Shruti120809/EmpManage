using EmpManage.DTOs;
using EmpManage.Helper;
using EmpManage.Interfaces;
using EmpManage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace EmpManage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PermissionController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public PermissionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetPermission")]
        public async Task<ResponseDTO<List<PermissionDTO>>> GetPermissions()
        {
            Log.Information("GetPermissions called");

            var result = await _unitOfWork.AddPermission.GetPermissionsAsync();

            Log.Information("Fetched {Count} permissions", result.Count);

            return new ResponseDTO<List<PermissionDTO>>(
                200,
                ResponseHelper.Success("fetched", "Permissions"),
                result
            );
        }

        [HttpGet("GetPermissionByRole/{roleId}")]

        public async Task<ResponseDTO<List<RoleMenuPermission>>> GetPermissionByRole(int roleId)
        {
            Log.Information("GetPermissionByRole called for RoleId: {RoleId}", roleId);

            var result = await _unitOfWork.AddPermission.GetPermissionsByRoleAsync(roleId);

            Log.Information("Fetched {Count} permissions for RoleId: {RoleId}", result.Count, roleId);

            return new ResponseDTO<List<RoleMenuPermission>>(
                200,
                ResponseHelper.Success("fetched", "Permissions"),
                result
            );
        }

        [HttpPost("ManagePermission")]
        public async Task<ResponseDTO<object>> ManagePermission([FromBody] AddPermissionDTO dto)
        {
            Log.Information("ManagePermission called for RoleId: {RoleId}, MenuId: {MenuId}", dto.RoleId, dto.MenuId);

            await _unitOfWork.AddPermission.AssignAsyncPermission(dto);
            await _unitOfWork.CompleteAsync();

            Log.Information("Permission assigned successfully for RoleId: {RoleId}, MenuId: {MenuId}", dto.RoleId, dto.MenuId);

            return new ResponseDTO<object>(
                200,
                ResponseHelper.PermissionAssigned(dto.MenuId, dto.RoleId),
                dto
            );
        }

    }
}