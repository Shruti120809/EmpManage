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

        [HttpGet("GetData")]
        public async Task<IActionResult> GetProfile()
        {
            int empId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var employee = await _unitofwork.Employee.GetByIdAsync(empId);

            if (employee == null)
                return NotFound(new ResponseDTO<object>(
                    404,
                    ResponseHelper.NotFound("User", employee),
                    null));

            var employeedto = _mapper.Map<EmployeeDTO>(employee);

            return Ok(new ResponseDTO<object>(
                200,
                ResponseHelper.Success("fetched", "User Profile"),
                employeedto));
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateProfile(UpdateDTO updatedto)
        {
            int empId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var existingEmployee = await _unitofwork.Employee.GetByIdAsync(empId);

            if (existingEmployee == null || existingEmployee.IsDeleted)
            {
                return NotFound(new ResponseDTO<object>(
                    404,
                    ResponseHelper.NotFound("User", empId),
                    null));
            }

            await _unitofwork.Employee.UpdateAsync(empId, updatedto, User);
            await _unitofwork.CompleteAsync();

            return Ok(new ResponseDTO<object>(
                200,
                ResponseHelper.Updated("User", empId),
                null));
        }

        [HttpDelete("DeleteData")]
        public async Task<IActionResult> DeleteOwnAccount()
        {
            int empId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _unitofwork.Employee.DeleteAsync(empId);
            await _unitofwork.CompleteAsync();

            return Ok(new ResponseDTO<object>(
                200,
                ResponseHelper.Deleted("User", empId),
                null));
        }
    }
}
