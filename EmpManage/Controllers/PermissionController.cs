using EmpManage.DTOs;
using EmpManage.Helper;
using EmpManage.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace EmpManage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles ="Admin")]
    public class PermissionController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        public PermissionController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetPermissionByRole/{roleId}")]
        public async Task<ResponseDTO<object>> GetPermissionByRole(int roleId)
        {
            var result = await _unitOfWork.AddPermission.GetPermissionsByRoleAsync(roleId);
            return (new ResponseDTO<object>(
                200,
                ResponseHelper.Success("Fetched", "Permissions"),
                result));
        }

        [HttpPost("AssignPermission")]
        public async Task<ResponseDTO<object>> AssignPermission([FromBody] AddPermissionDTO addPermissionDTO)
        {
            await _unitOfWork.AddPermission.AssignAsyncPermission(addPermissionDTO);
            await _unitOfWork.CompleteAsync();
            return new ResponseDTO<object>(
                200,
                ResponseHelper.Assigned("Permissions", addPermissionDTO.MenuName, addPermissionDTO.RoleName),
                null);
        }

        [HttpPost("RemovePermission")]
        public async Task<ResponseDTO<object>> RemovePermission([FromQuery]int roleId, [FromQuery]int menuId)
        {
            await _unitOfWork.AddPermission.RemoveAsyncPermission(roleId, menuId);
            await _unitOfWork.CompleteAsync();
            return new ResponseDTO<object>(
                200,
                ResponseHelper.Removed("Permission", menuId, roleId),
                null);
        }
    }
}
