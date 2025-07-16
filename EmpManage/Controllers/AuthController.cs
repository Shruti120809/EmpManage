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
        private readonly IUnitOfWork _unitofwork;
        public AuthController( IUnitOfWork unitOfwork) 
        { 
            _unitofwork = unitOfwork;
        }

        [HttpPost("Register")]
        public async Task<ResponseDTO<object>> Register([FromBody] RegisterDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return new ResponseDTO<object>(
                    400,
                    "Validation Failed",
                    errors);
            }

            var Name = dto.Name;
            if ( Name == null || Name == "" || Name.All(char.IsWhiteSpace) || Name != Name.Trim())
                {
                    return new ResponseDTO<object>(
                        400,
                        "Validation Failed",
                        new List<string> { "Name cannot be empty, contain only spaces, or start/end with spaces." });
                }

            var email = dto.Email.Trim().ToLower();

            if (await _unitofwork.Auth.UserExists(email))
            {
                return new ResponseDTO<object>(
                    409,
                    "Email already exists.",
                    null);
            }
            var newUser = await _unitofwork.Auth.RegisterAsync(dto, User);
            await _unitofwork.CompleteAsync();

            return new ResponseDTO<object>(
                200,
                "User registered successfully.",
                new { newUser.Id, newUser.Email });
        }

        [HttpPost("login")]
        public async Task<ResponseDTO<object>> Login (LoginDTO logindto)
        {
            var token = await _unitofwork.Auth.LoginAsync(logindto);

            if (token == null)
                return new ResponseDTO<object>(
                    401,
                    ResponseHelper.Unauthorized(),
                    null );

            return  (new ResponseDTO<object>(
                200,
                ResponseHelper.LoggedIn("User"),
                new { token }));
        }
    }
}
