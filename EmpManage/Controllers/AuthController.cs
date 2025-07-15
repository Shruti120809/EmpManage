using EmpManage.Data;
using EmpManage.DTOs;
using EmpManage.Helper;
using EmpManage.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        public async Task<IActionResult> Register ([FromBody] RegisterDTO registerdto)
        {
            if (await _authrepo.UserExists(registerdto.Email))
                return BadRequest(new ResponseDTO<object>(
                    400,
                    ResponseHelper.AlreadyExists("Email"),
                    null));

            var newUser = await _authrepo.RegisterAsync(registerdto, User);
            return Ok(new ResponseDTO<object>(
                    200,
                    ResponseHelper.Fetched("User", newUser.Id),
                    new { newUser.Id, newUser.Email }
                ));
        } 

        [HttpPost("login")]
        public async Task<IActionResult> Login (LoginDTO logindto)
        {
            var token = await _authrepo.LoginAsync(logindto);

            if (token == null)
                return Unauthorized(new ResponseDTO<object>(
                    401,
                    ResponseHelper.Unauthorized(),
                    null ));

            return Ok(new ResponseDTO<object>(
                200,
                ResponseHelper.LoggedIn("User"),
                new { token }));
        }
    }
}
