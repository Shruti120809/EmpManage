using AutoMapper;
using EmpManage.DTOs;
using EmpManage.Helper;
using EmpManage.Interfaces;
using EmpManage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;
using Serilog;

namespace EmpManage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Intern,Hr,Manager,Admin")]
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
            Log.Information("GetAll called with {@PaginationDTO}", dto);

            try
            {
                
                var employees = await _unitOfWork.Employee.GetAllAsync(dto);
                Log.Information("GetAll successfully retrieved {Count} employees", employees?.Items?.Count ?? 0);

                return new ResponseDTO<PaginationDTO<EmployeeDTO>>(
                    200,
                    ResponseHelper.Success("fetched", "Data"),
                    employees);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while fetching employees");
                throw;
            }
        }

        [HttpGet("GetById/{id}")]
        public async Task<ResponseDTO<EmployeeDTO>> GetById(int id)
        {
            Log.Information("GetById called for EmployeeId: {Id}", id);

            try
            {
                var employee = await _unitOfWork.Employee.GetByIdAsync(id);

                if (employee == null)
                {
                    Log.Warning("Employee with Id {Id} not found", id);
                    return new ResponseDTO<EmployeeDTO>(
                        404,
                        ResponseHelper.NotFound("Employee"),
                        null);
                }

                Log.Information("Employee with Id {Id} fetched successfully", id);
                return new ResponseDTO<EmployeeDTO>(
                    200,
                    ResponseHelper.Success("fetched", "Employee"),
                    employee);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while fetching EmployeeId: {Id}", id);
                throw;
            }
        }

        [HttpPut("UpdateById/{id}")]
        public async Task<ResponseDTO<UpdateDTO>> UpdateById(int id, [FromBody] UpdateDTO dto)
        {
            Log.Information("UpdateById called for EmployeeId: {Id} with {@UpdateDTO}", id, dto);

            if (!ModelState.IsValid)
            {
                Log.Warning("Invalid update request for EmployeeId: {Id}", id);
                return new ResponseDTO<UpdateDTO>(
                    400,
                    ResponseHelper.BadRequest("Invalid input data."),
                    null);
            }

            try
            {
                var isUpdated = await _unitOfWork.Employee.UpdateByIdAdminAsync(id, dto);

                if (isUpdated == null)
                {
                    Log.Warning("Employee with Id {Id} not found for update", id);
                    return new ResponseDTO<UpdateDTO>(
                        404,
                        ResponseHelper.NotFound("Employee"),
                        null);
                }

                Log.Information("Employee with Id {Id} updated successfully", id);
                return new ResponseDTO<UpdateDTO>(
                    200,
                    ResponseHelper.Success("Employee", "updated"),
                    isUpdated);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while updating EmployeeId: {Id}", id);
                throw;
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<ResponseDTO<DeleteDTO>> DeleteById(int id)
        {
            Log.Information("DeleteById called for EmployeeId: {Id}", id);

            try
            {
                var deletedEmployee = await _unitOfWork.Employee.DeleteAsync(id);

                if (deletedEmployee == null)
                {
                    Log.Warning("Employee with Id {Id} not found for deletion", id);
                    return new ResponseDTO<DeleteDTO>(
                        404,
                        ResponseHelper.NotFound("Employee"),
                        null);
                }

                Log.Information("Employee with Id {Id} deleted successfully", id);
                return new ResponseDTO<DeleteDTO>(
                    200,
                    ResponseHelper.Success("Employee", "deleted"),
                    deletedEmployee);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while deleting EmployeeId: {Id}", id);
                throw;
            }
        }

        [HttpPost("MimicUser")]
        public async Task<ResponseDTO<MimicResponseDTO>> MimicUser([FromBody] MimicUserDTO user)
        {
            Log.Information("Mimic User called");

            try
            {
                var employee = await _unitOfWork.Employee.MimicUserAsync(user);
                Log.Information("Retrieved User");

                return new ResponseDTO<MimicResponseDTO>(
                    200,
                    ResponseHelper.Success("user", "Mimic"),
                    employee); // ✅ Pass the correct type
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in MimicUser");
                return new ResponseDTO<MimicResponseDTO>(
                    500,
                    ResponseHelper.InternalError(ex.Message),
                    null);
            }
        }


        [HttpGet("GetAllRoles")]
        public async Task<ResponseDTO<List<RoleDTO>>> GetAllRoles()
        {
            Log.Information("GetAllRoles called");

            try
            {
                var roles = await _unitOfWork.Employee.GetAllRoleAsync();
                Log.Information("Retrieved {Count} roles", roles.Count);

                return new ResponseDTO<List<RoleDTO>>(
                    200,
                    ResponseHelper.Success("roles", "Get all"),
                    roles);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while fetching roles");
                throw;
            }
        }

        [HttpPost("ChangeRole")]
        public async Task<ResponseDTO<UpdateRoleDTO>> UpdateRoles([FromBody] UpdateRoleDTO dto)
        {
            Log.Information("ChangeRole called with {@UpdateRoleDTO}", dto);

            try
            {
                var result = await _unitOfWork.Employee.UpdateRolesAsync(dto);
                Log.Information("Roles updated successfully");

                return new ResponseDTO<UpdateRoleDTO>(
                    200,
                    ResponseHelper.Success("Update", "Roles"),
                    dto);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while updating roles");
                throw;
            }
        }
    }
}