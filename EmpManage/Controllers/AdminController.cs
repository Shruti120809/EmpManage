using EmpManage.DTOs;
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
        private readonly IEmployeeRepository _emprepo;

        public AdminController(IEmployeeRepository emprepo)
        {
            _emprepo = emprepo;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _emprepo.GetAllAsync();
            var result = users.Select(u => new
            {
                id = u.Id,
                name = u.Name,
                email = u.Email,
                Roles = u.EmpRoles?.Select(r => r.Role?.Name).ToList()
            });

            return Ok(result);
        }

        [HttpPost("AssignRoles")]
        public async Task<IActionResult> AssignRole(AssignRoleDTO assignrole)
        {
            await _emprepo.AssignRoleAsync(assignrole);
            return Ok("Roles Assigned");
        }

        [HttpPost("RemoveRole")]
        public async Task<IActionResult> RemoveRole(RemoveRoleDTO dto)
        {
            await _emprepo.RemoveRoleAsync(dto);
            return Ok("Role removed successfully.");
        }


        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _emprepo.GetByIdAsync(id);
            if (user == null) return BadRequest("Account Not Found");
            return Ok(new
            {
                id = user.Id,
                name = user.Name,
                email = user.Email,
                Roles = user.EmpRoles?.Select(r => r.Role?.Name).ToList()
            });
        }

        [HttpPost("UpdateById")]
        public async Task<IActionResult> UpdateById(int id, UpdateDTO updatedto)
        {
            await _emprepo.UpdateByIdAdminAsync(id, updatedto);
            return Ok("User updated successfully.");
        }

        [HttpDelete("DeleteById")]
        public async Task<IActionResult> DeleteById(int id)
        {
            await _emprepo.DeleteAsync(id);
            return Ok("Account Deleted");
        }
    }
}
