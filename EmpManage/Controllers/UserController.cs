using AutoMapper;
using EmpManage.DTOs;
using EmpManage.Helper;
using EmpManage.Interfaces;
using EmpManage.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Serilog;

namespace EmpManage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "User,Intern,Hr,Manager,Admin")]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitofwork;
        private readonly IMapper _mapper;

        public UserController(IUnitOfWork unitofwork, IMapper mapper)
        {
            _unitofwork = unitofwork;
            _mapper = mapper;
        }

        [HttpGet("Profile")]
        public async Task<ResponseDTO<EmployeeDTO>> GetProfile()
        {
            var empId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            Log.Information("GetProfile called by UserId: {EmpId}", empId);

            var result = await _unitofwork.Employee.GetByIdAsync(empId);

            if (result == null)
            {
                Log.Warning("Profile not found for UserId: {EmpId}", empId);
                return new ResponseDTO<EmployeeDTO>(
                    404,
                    ResponseHelper.NotFound("Employee"),
                    null);
            }

            Log.Information("Profile fetched successfully for UserId: {EmpId}", empId);
            return new ResponseDTO<EmployeeDTO>(
                200,
                ResponseHelper.Success("Profile", "Fetched"),
                result);
        }

        [HttpPut("UpdateProfile")]
        public async Task<ResponseDTO<UpdateDTO>> UpdateProfile([FromBody] UpdateDTO dto)
        {
            var empId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            Log.Information("UpdateProfile called by UserId: {EmpId} with {@UpdateDTO}", empId, dto);

            if (!ModelState.IsValid)
            {
                Log.Warning("Invalid model state for UserId: {EmpId}", empId);
                return new ResponseDTO<UpdateDTO>(
                    400,
                    ResponseHelper.ValidationError(ModelState),
                    dto);
            }

            try
            {
                await _unitofwork.Employee.UpdateAsync(empId, dto, User);
                Log.Information("Profile updated successfully for UserId: {EmpId}", empId);

                return new ResponseDTO<UpdateDTO>(
                    200,
                    ResponseHelper.Success("updated", "Profile"),
                    dto);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error updating profile for UserId: {EmpId}", empId);
                throw;
            }
        }
    }
}