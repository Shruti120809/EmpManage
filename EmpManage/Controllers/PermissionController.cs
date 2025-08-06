using EmpManage.DTOs;
using EmpManage.Helper;
using EmpManage.Interfaces;
using EmpManage.Models;
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
        public async Task<ResponseDTO<List<RoleMenuPermission>>> GetPermissionByRole(int roleId)
        {
            var result = await _unitOfWork.AddPermission.GetPermissionsByRoleAsync(roleId);

            return new ResponseDTO<List<RoleMenuPermission>>(
                200,
                ResponseHelper.Success("fetched", "Permissions"),
                result
            );
        }


        [HttpPost("ManagePermission")]
        public async Task<ResponseDTO<object>> ManagePermission([FromBody] AddPermissionDTO dto)
        {
            await _unitOfWork.AddPermission.AssignAsyncPermission(dto);
            await _unitOfWork.CompleteAsync();
            return new ResponseDTO<object>(
                200,
                ResponseHelper.PermissionAssigned(dto.MenuId,dto.RoleId),
                dto);
        }

        //[HttpPost("RemovePermission")]
        //public async Task<ResponseDTO<object>> RemovePermission([FromQuery]int roleId, [FromQuery]int menuId)
        //{
        //    await _unitOfWork.AddPermission.RemoveAsyncPermission(roleId, menuId);
        //    await _unitOfWork.CompleteAsync();
        //    return new ResponseDTO<object>(
        //        200,
        //        ResponseHelper.Removed("Permission", menuId, roleId),
        //        null);
        //}
    }
}
