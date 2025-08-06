using EmpManage.Data;
using EmpManage.DTOs;
using EmpManage.Helper;
using EmpManage.Interfaces;
using EmpManage.Models;
using EmpManage.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


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

        //URL: https://localhost:7212/api/Auth/Register
        [HttpPost("Register")]
        public async Task<ResponseDTO<Employee>> Register([FromBody] RegisterDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return new ResponseDTO<Employee>(
                    400,
                    ResponseHelper.ValidationError(ModelState),
                    null);
            }
            var name = dto.Name?.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                return new ResponseDTO<Employee>(
                    400,
                    ResponseHelper.BadRequest("Name field"),
                    null);
            }
            var email = dto.Email?.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(email))
            {
                return new ResponseDTO<Employee>(
                    400,
                    ResponseHelper.BadRequest("Email field"),
                    null);
            }
            if (await _unitofwork.Auth.UserExists(email))
            {
                return new ResponseDTO<Employee>(
                    409,
                    ResponseHelper.Exists("User"),
                    null);
            }
            var newUser = await _unitofwork.Auth.RegisterAsync(dto, User);
            if (newUser == null)
            {
                return new ResponseDTO<Employee>(
                    500,
                    ResponseHelper.InternalError("User"),
                    null);
            }
            await _unitofwork.CompleteAsync();
            return new ResponseDTO<Employee>(
                200,
                ResponseHelper.Success("registered", "User"),
                newUser);
        }

        //URL: https://localhost:7212/api/Auth/Login
        [HttpPost("Login")]
        public async Task<ResponseDTO<LoginResponseDTO>> Login(LoginDTO logindto)
        {
            var response = await _unitofwork.Auth.LoginAsync(logindto);

            if (response == null)
                return new ResponseDTO<LoginResponseDTO>(
                    401,
                    ResponseHelper.Unauthorized(),
                    null);

            return new ResponseDTO<LoginResponseDTO>(
                200,
                ResponseHelper.LoggedIn("User"),
                response);
        }

        //URL: https://localhost:7212/api/Auth/ForgotPassword
        [HttpPost("ForgotPassword")]
        public async Task<ResponseDTO<bool>> ForgotPassword([FromBody] ForgetPasswordDTO dto)
        {
            var success = await _unitofwork.Auth.GenerateResetPasswordAsync(new Employee { Email = dto.Email });
            if (!success)
            {
                return new ResponseDTO<bool>(
                    404,
                    ResponseHelper.NotFound("User"),
                    false);
            }
            return new ResponseDTO<bool>(
                200,
                ResponseHelper.Success("sent", "OTP"),
                true);
        }

        [HttpPost("VerifyOtp")]
        public async Task<ResponseDTO<Employee>> VerifyOtp([FromBody] VerifyOtpDTO dto)
        {
            var employee = await _unitofwork.Auth.VerifyOtpAsync(dto.Email, dto.Otp);
            if (employee == null)
            {
                return new ResponseDTO<Employee>(
                    400,
                    ResponseHelper.Invalid("OTP or OTP expired"),
                    null);
            }
            return new ResponseDTO<Employee>(
                200,
                ResponseHelper.Success("verified", "OTP"),
                employee);
        }

        [HttpPost("ResetPassword")]
        public async Task<ResponseDTO<bool>> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword)
            {
                return new ResponseDTO<bool>(
                    400,
                    ResponseHelper.Mismatch("Passwords"),
                    false);
            }

            var user = await _unitofwork.Auth.GetUserByEmailAsync(dto.Email);
            if (user == null)
            {
                return new ResponseDTO<bool>(
                    404,
                    ResponseHelper.NotFound("User"),
                    false);
            }

            // Hash new password to compare with old one
            using var hmac = new HMACSHA512();
            var newPasswordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.NewPassword)));

            if (user.Password == newPasswordHash)
            {
                return new ResponseDTO<bool>(
                    400,
                    "New password cannot be same as the old password.",
                    false);
            }

            // Proceed to update password
            var success = await _unitofwork.Auth.ResetPasswordAsync(dto.Email, dto.NewPassword);
            if (!success)
            {
                return new ResponseDTO<bool>(
                    500,
                    "Something went wrong while resetting the password.",
                    false);
            }

            return new ResponseDTO<bool>(
                200,
                ResponseHelper.Success("reset", "Password"),
                true);

        }


    }
}