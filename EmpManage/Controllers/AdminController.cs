using AutoMapper;
using EmpManage.DTOs;
using EmpManage.Helper;
using EmpManage.Interfaces;
using EmpManage.Models;
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

        [HttpPost("GetAll")]
        public async Task<ResponseDTO<PaginationDTO<EmployeeDTO>>> GetAll([FromBody] SortingPaginationDTO dto)
        {
            var employees = await _unitOfWork.Employee.GetAllAsync(dto);

            return new ResponseDTO<PaginationDTO<EmployeeDTO>>(
                200,
                ResponseHelper.Success("Fetched", "successfully"),
                employees);
        }

        [HttpGet("GetById/{id}")]
        public async Task<ResponseDTO<EmployeeDTO>> GetById(int id)
        {
            var employee = await _unitOfWork.Employee.GetByIdAsync(id);

            if (employee == null)
                return new ResponseDTO<EmployeeDTO>(
                    404,
                    ResponseHelper.NotFound("Employee"),
                    null);

            return new ResponseDTO<EmployeeDTO>(
                200,
                ResponseHelper.Success("fetched", "Employee"),
                employee);
        }

        [HttpPut("UpdateById/{id}")]
        public async Task<ResponseDTO<UpdateDTO>> UpdateById(int id, [FromBody] UpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return new ResponseDTO<UpdateDTO>(
                    400,
                    ResponseHelper.BadRequest("Invalid input data."),
                    null);

            var isUpdated = await _unitOfWork.Employee.UpdateByIdAdminAsync(id, dto);

            if (isUpdated == null)
                return new ResponseDTO<UpdateDTO>(
                    404,
                    ResponseHelper.NotFound("Employee"),
                    null);

            return new ResponseDTO<UpdateDTO>(
                200,
                ResponseHelper.Success("Employee", "updated"),
                isUpdated);
        }   

        [HttpDelete("Delete/{id}")]
        public async Task<ResponseDTO<DeleteDTO>> DeleteById(int id)
        {
            var dto = new Employee
            {
                UpdatedBy = "System"
            };

            var deletedEmployee = await _unitOfWork.Employee.DeleteAsync(id);

            if (deletedEmployee == null)
            {
                return new ResponseDTO<DeleteDTO>(
                    404,
                    ResponseHelper.NotFound("Employee"),
                    null);
            }

            return new ResponseDTO<DeleteDTO>(
                200,
                ResponseHelper.Success("Employee", "deleted"),
                deletedEmployee);
        }


        [HttpPost("AssignRole")]
        public async Task<ResponseDTO<AssignRoleDTO>> AssignRole(AssignRoleDTO dto)
        {
            var result = await _unitOfWork.Employee.AssignRoleAsync(dto);

            if (result == null)
            {
                return new ResponseDTO<AssignRoleDTO>(
                    404,
                    ResponseHelper.NotFound("User or Role"),
                    null
                );
            }
            return new ResponseDTO<AssignRoleDTO>(
                200,
                ResponseHelper.Success("Role", "assigned"),
                result
            );
        }

        [HttpDelete("RemoveRole")]
        public async Task<ResponseDTO<object>> RemoveRole(RemoveRoleDTO dto)
        {
            var result = await _unitOfWork.Employee.RemoveRoleAsync(dto);

            if (result == null)
                return new ResponseDTO<object>(
                    404,
                    ResponseHelper.NotFound("Role or User"),
                    null);

            return new ResponseDTO<object>(
                200, 
                ResponseHelper.Removed("Role","from","to"),
                dto);
        }


        #region Linq
        /*
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
        */
        #endregion
    }
}