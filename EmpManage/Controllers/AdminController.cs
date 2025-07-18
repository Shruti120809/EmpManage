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
        public async Task<ResponseDTO<object>> GetAll()
        {
            var employees = await _unitOfWork.Employee.GetAllAsync();

            if (employees == null || !employees.Any())
            {
               return new ResponseDTO<object>(
                    404,
                    ResponseHelper.NotFound("User or Role"),
                    null);
            }

            var employeeDtos = employees.Select(e => _mapper.Map<EmployeeDTO>(e)).ToList();

            return new ResponseDTO<object>(
                200,
                ResponseHelper.Success("Fetched", "All Users"),
                employeeDtos);
        }


        [HttpPost("AssignRoles")]
        public async Task<ResponseDTO<object>> AssignRole(AssignRoleDTO assignrole)
        {
            var user = await _unitOfWork.Employee.GetByIdAsync(assignrole.EmployeeId);
            var roles = await _unitOfWork.Employee.GetRolesByIdsAsync(assignrole.RoleId);

            if (user == null || roles == null)
            {
                return new ResponseDTO<object>(
                    404,
                    ResponseHelper.NotFound("User or Role"),
                    null);
            }

            await _unitOfWork.Employee.AssignRoleAsync(assignrole);

            return new ResponseDTO<object>(
                200,
                ResponseHelper.Assigned("Roles", string.Join(", ", roles.Select(r => r.Name)), user.Name),
                null);
        }

        [HttpPost("RemoveRoles")]
        public async Task<ResponseDTO<object>> RemoveRoles(RemoveRoleDTO dto)
        {
            var user = await _unitOfWork.Employee.GetByIdAsync(dto.EmployeeId);
            var roles = await _unitOfWork.Employee.GetRolesByIdsAsync(dto.RoleIds);

            if (user == null || roles == null || !roles.Any())
            {
                return new ResponseDTO<object>(
                    404,
                    ResponseHelper.NotFound("User or Roles"),
                    null);
            }

            await _unitOfWork.Employee.RemoveRoleAsync(dto);

            return new ResponseDTO<object>(
                200,
                ResponseHelper.Removed("Roles", string.Join(", ", roles.Select(r => r.Name)), user.Name),
                null);
        }

        [HttpGet("Get/{id}")]
        public async Task<ResponseDTO<object>> GetById(int id)
        {
            var user = await _unitOfWork.Employee.GetByIdAsync(id);

            if (user == null)
            {
                return new ResponseDTO<object>(
                    404,
                    ResponseHelper.NotFound("User"),
                    null);
            }

            var employeeDto = _mapper.Map<EmployeeDTO>(user);

            return new ResponseDTO<object>(
                200,
                ResponseHelper.Retrieved("User", id),
                employeeDto);
        }


        [HttpPut("Update/{id}")]
        public async Task<ResponseDTO<object>> UpdateById(int id, UpdateDTO updatedto)
        {
            await _unitOfWork.Employee.UpdateByIdAdminAsync(id, updatedto);
            return new ResponseDTO<object>(
                200,
                ResponseHelper.Updated("User", id),
                null);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<ResponseDTO<object>> DeleteById(int id)
        {
            await _unitOfWork.Employee.DeleteAsync(id);
            return new ResponseDTO<object>(
                200,
                ResponseHelper.Deleted("User", id),
                null);
        }
    }
}
