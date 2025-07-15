using AutoMapper;
using EmpManage.DTOs;
using EmpManage.Helper;
using EmpManage.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmpManage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AdminController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _unitOfWork.Employee.GetAllAsync();

            if (employees == null || !employees.Any())
            {
                return NotFound(new ResponseDTO<object>(
                    404,
                    ResponseHelper.NotFound("Users"),
                    null));
            }

            var employeeDtos = employees.Select(e => _mapper.Map<EmployeeDTO>(e)).ToList();

            return Ok(new ResponseDTO<object>(
                200,
                ResponseHelper.Success("fetched", "All Users"),
                employeeDtos)); ;
        }

        [HttpPost("AssignRoles")]
        public async Task<IActionResult> AssignRole(AssignRoleDTO assignrole)
        {
            var user = await _unitOfWork.Employee.GetByIdAsync(assignrole.EmployeeId);
            var roles = await _unitOfWork.Employee.GetRolesByIdsAsync(assignrole.RoleId);

            if (user == null || roles == null)
            {
                return NotFound(new ResponseDTO<object>(
                    404,
                    ResponseHelper.NotFound("User or Role", assignrole.EmployeeId),
                    null));
            }

            await _unitOfWork.Employee.AssignRoleAsync(assignrole);

            return Ok(new ResponseDTO<object>(
                200,
                ResponseHelper.Assigned("Roles", string.Join(", ", roles.Select(r => r.Name)), user.Name),
                null));
        }

        [HttpPost("RemoveRoles")]
        public async Task<IActionResult> RemoveRoles(RemoveRoleDTO dto)
        {
            var user = await _unitOfWork.Employee.GetByIdAsync(dto.EmployeeId);
            var roles = await _unitOfWork.Employee.GetRolesByIdsAsync(dto.RoleIds);

            if (user == null || roles == null || !roles.Any())
            {
                return NotFound(new ResponseDTO<object>(
                    404,
                    ResponseHelper.NotFound("User or Roles", dto.EmployeeId),
                    null));
            }

            await _unitOfWork.Employee.RemoveRoleAsync(dto);

            return Ok(new ResponseDTO<object>(
                200,
                ResponseHelper.Removed("Roles", string.Join(", ", roles.Select(r => r.Name)), user.Name),
                null));
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _unitOfWork.Employee.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound(new ResponseDTO<object>(
                    404,
                    ResponseHelper.NotFound("User", id),
                    null));
            }

            return Ok(new ResponseDTO<object>(
                200,
                ResponseHelper.Retrieved("User", id),
                new
                {
                    id = user.Id,
                    name = user.Name,
                    email = user.Email,
                    Roles = user.EmpRoles?.Select(r => r.Role?.Name).ToList()
                }));
        }

        [HttpPut("UpdateById")]
        public async Task<IActionResult> UpdateById(int id, UpdateDTO updatedto)
        {
            await _unitOfWork.Employee.UpdateByIdAdminAsync(id, updatedto);
            return Ok(new ResponseDTO<object>(
                200,
                ResponseHelper.Updated("User", id),
                null));
        }

        [HttpDelete("DeleteById")]
        public async Task<IActionResult> DeleteById(int id)
        {
            await _unitOfWork.Employee.DeleteAsync(id);
            return Ok(new ResponseDTO<object>(
                200,
                ResponseHelper.Deleted("User", id),
                null));
        }
    }
}
