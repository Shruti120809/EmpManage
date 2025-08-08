using EmpManage.Data;
using EmpManage.DTOs;
using EmpManage.Helper;
using EmpManage.Interfaces;
using EmpManage.Models;
using EmpManage.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;


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

        private bool IsValidEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return false;

                // Clean hidden characters
                email = email.Trim();                            
                email = Regex.Replace(email, @"\s+", "");       
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
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
            if (!IsValidEmail(dto.Email))
                return new ResponseDTO<bool>(
                    400,
                    ResponseHelper.BadRequest("Invalid"),
                    false);

            return new ResponseDTO<bool>(
                200,
                ResponseHelper.Success("sent", "OTP"),
                true);

        }

        [HttpPost("VerifyOtp")]
        public async Task<ResponseDTO<string>> VerifyOtp([FromBody] VerifyOtpDTO dto)
        {
            var token = await _unitofwork.Auth.VerifyOtpAsync(dto);

            if (token == null)
                return new ResponseDTO<string>(
                        400,
                        ResponseHelper.Invalid("Otp"),
                        dto.Otp.ToString()
                );

            return new ResponseDTO<string>(
                200,
                ResponseHelper.Success("generated", "Token"),
                token);
        }

        [HttpPost("ResetPassword")]
        public async Task<ResponseDTO<bool>> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword)
            {
                return new ResponseDTO<bool>(400, "Passwords do not match", false);
            }

            if (!Guid.TryParse(dto.Token, out var parsedToken))
            {
                return new ResponseDTO<bool>(400, "Invalid token format", false);
            }

            var result = await _unitofwork.Auth.ResetPasswordAsync(parsedToken, dto.NewPassword);
            if (!result)
            {
                return new ResponseDTO<bool>(404, "Token is invalid or expired", false);
            }

            return new ResponseDTO<bool>(200, "Password reset successful", true);
        }


    }
}