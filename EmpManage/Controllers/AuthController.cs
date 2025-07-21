using EmpManage.Data;
using EmpManage.DTOs;
using EmpManage.Helper;
using EmpManage.Interfaces;
using EmpManage.Repositories;
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

        [HttpPost("Login")]
        public async Task<ResponseDTO<object>> Login(LoginDTO logindto)
        {
            var response = await _unitofwork.Auth.LoginAsync(logindto);

            if (response == null)
                return new ResponseDTO<object>(
                    401,
                    ResponseHelper.Unauthorized(),
                    null);

            return new ResponseDTO<object>(
                200,
                ResponseHelper.LoggedIn("User"),
                response);
        }

        [HttpPost("ForgotPassword")]
        public async Task<ResponseDTO<object>> ForgetPassword(ForgetPasswordDTO dto)
        {
            var user = await _unitofwork.Auth.GetUserByEmailAsync(dto.Email);
            if (user == null)
                return new ResponseDTO<object>(
                    404,
                    ResponseHelper.NotFound("User"),
                    null);

            var token = await _unitofwork.Auth.GenerateResetPasswordAsync(user);
            var resetLink = $"http://localhost:5000/reset-password?token={token}";

            await _unitofwork.Email.SendEmailAsync(
                user.Email,
                "Password Reset Request",
                $"<p>Click the link to reset your password:</p><a href='{resetLink}'>{resetLink}</a>"
            );

            return new ResponseDTO<object>(
                    200,
                    ResponseHelper.Success("OTP","Sent"),
                    null);
        }

        [HttpPost("ResetPassword")]
        public async Task<ResponseDTO<object>> ResetPassword(ResetPasswordDTO dto)
        {
            var result = await _unitofwork.Auth.ResetPasswordAsync(dto.Email, dto.Otp, dto.Password);

            return result ?
                new ResponseDTO<object>(
                    200,
                    ResponseHelper.Success("Password","Reset"),
                    null
                 ) :
                 new ResponseDTO<object>(
                     400,
                     ResponseHelper.BadRequest("OTP"),
                     null
                 );

        }
    }
}