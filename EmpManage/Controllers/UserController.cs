using EmpManage.DTOs;
using EmpManage.Interfaces;
using EmpManage.Models;
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
        private readonly IEmployeeRepository _employeerepo;

        public UserController (IEmployeeRepository emprepo)
        {
            _employeerepo = emprepo;
        }

        [HttpGet("GetData")]
        public async Task<IActionResult> GetProfile() {
            int empId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var employee = await _employeerepo.GetByIdAsync(empId);

            if(employee == null) return NotFound("Employee Doesn't Exist");

            return Ok(new
            {
                employee.Id,
                employee.Name,
                employee.Email,
                Roles = employee.EmpRoles?.Select(r => r.Role?.Name).ToList()
            });
        }

        [HttpPut("Update")]
        public async Task<IActionResult> UpdateProfile(UpdateDTO updatedto)
        {
            int empId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _employeerepo.UpdateAsync(empId, updatedto);
            return Ok("Profile Gets Updated");
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteOwnAccount()
        {
            int empId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _employeerepo.DeleteAsync(empId);
            return Ok("Your account has been deleted.");
        }

    }
}