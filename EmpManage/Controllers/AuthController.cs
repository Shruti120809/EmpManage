using EmpManage.Data;
using EmpManage.DTOs;
using EmpManage.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmpManage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authrepo;
        public AuthController( IAuthRepository authrepo) 
        { 
            _authrepo = authrepo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register (RegisterDTO registerdto)
        {
            if (await _authrepo.UserExists(registerdto.Email))
                return BadRequest("Email Already Registered");

            var newUser = await _authrepo.RegisterAsync(registerdto);
            return Ok(new { message = "Registration successful", newUser.Id, newUser.Email });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login (LoginDTO logindto)
        {
            var token = await _authrepo.LoginAsync(logindto);

            if (token == null)
                return Unauthorized("Invalid email or password.");

            return Ok(new { token });
        }
    }
}
