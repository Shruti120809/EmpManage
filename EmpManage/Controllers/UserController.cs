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
        public async Task<ResponseDTO<object>> GetProfile()
        {
            int empId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var employee = await _unitofwork.Employee.GetByIdAsync(empId);

            if (employee == null)
                return (new ResponseDTO<object>(
                    404,
                    ResponseHelper.NotFound("User"),
                    null));

            var employeedto = _mapper.Map<EmployeeDTO>(employee);

            return (new ResponseDTO<object>(
                200,
                ResponseHelper.Success("fetched", "User Profile"),
                employeedto));
        }

        [HttpPut("Update/{id}")]
        public async Task<ResponseDTO<object>> UpdateProfile(int id, [FromBody] UpdateDTO updatedto)
        {

            int currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (id != currentUserId)
            {
                return new ResponseDTO<object>(
                    403,
                    "You are not authorized to update this profile.",
                    null);
            }

            var existingEmployee = await _unitofwork.Employee.GetByIdAsync(id);
            if (existingEmployee == null || existingEmployee.IsDeleted)
            {
                return new ResponseDTO<object>(
                    404,
                    ResponseHelper.NotFound("User"),
                    null);
            }

            await _unitofwork.Employee.UpdateAsync(id, updatedto, User);
            await _unitofwork.CompleteAsync();

            return new ResponseDTO<object>(
                200,
                ResponseHelper.Updated("User", id),
                null);
        }



        //[HttpDelete("DeleteData")]
        //public async Task<ResponseDTO<object>> DeleteOwnAccount()
        //{
        //    int empId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        //    await _unitofwork.Employee.DeleteAsync(empId);
        //    await _unitofwork.CompleteAsync();

        //    return (new ResponseDTO<object>(
        //        200,
        //        ResponseHelper.Deleted("User", empId),
        //        null));
        //}
    }
}
