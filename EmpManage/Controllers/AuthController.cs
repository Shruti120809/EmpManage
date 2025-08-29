using Azure;
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
using Serilog;

namespace EmpManage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitofwork;

        public AuthController(IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;
        }

        [HttpPost("Register")]
        public async Task<ResponseDTO<EmployeeDTO>> Register([FromBody] RegisterDTO dto)
        {
            Log.Information("Register called with {@RegisterDTO}", dto);

            if (!ModelState.IsValid)
            {
                Log.Warning("Register failed due to invalid model state");
                return new ResponseDTO<EmployeeDTO>(400, ResponseHelper.ValidationError(ModelState), null);
            }

            var email = dto.Email?.Trim().ToLower();

            if (await _unitofwork.Auth.UserExists(email))
            {
                Log.Warning("Register attempt with existing email: {Email}", email);
                return new ResponseDTO<EmployeeDTO>(409, ResponseHelper.Exists("User"), null);
            }

            try
            {
                var newUser = await _unitofwork.Auth.RegisterAsync(dto);
                if (newUser == null)
                {
                    Log.Error("Register failed internally for email: {Email}", email);
                    return new ResponseDTO<EmployeeDTO>(500, ResponseHelper.InternalError("User"), null);
                }

                await _unitofwork.CompleteAsync();
                Log.Information("User registered successfully with email: {Email}", email);
                return new ResponseDTO<EmployeeDTO>(200, ResponseHelper.Success("registered", "User"), newUser);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred while registering email: {Email}", email);
                throw;
            }
        }

        [HttpPost("Login")]
        public async Task<IResponseDTO> Login(LoginDTO logindto)
        {
            Log.Information("Login called for email: {Email}", logindto.Email);

            try
            {
                var response = await _unitofwork.Auth.LoginAsync(logindto);

                if (response == null)
                {
                    Log.Warning("Unauthorized login attempt for email: {Email}", logindto.Email);
                    return new ResponseDTO<LoginResponseDTO>(401, ResponseHelper.Unauthorized(), null);
                }

                Log.Information("User logged in successfully: {Email}", logindto.Email);
                return new ResponseDTO<LoginResponseDTO>(200, ResponseHelper.LoggedIn("User"), response);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception occurred during login for email: {Email}", logindto.Email);
                return new ResponseDTO<string>(400, ResponseHelper.InternalError("User"), ex.Message);
            }
        }

        [HttpPost("ForgotPassword")]
        public async Task<ResponseDTO<bool>> ForgotPassword([FromBody] ForgetPasswordDTO dto)
        {
            Log.Information("ForgotPassword called for email: {Email}", dto.Email);

            if (!IsValidEmail(dto.Email))
            {
                Log.Warning("Invalid email format for forgot password: {Email}", dto.Email);
                return new ResponseDTO<bool>(400, ResponseHelper.BadRequest("Invalid"), false);
            }

            var success = await _unitofwork.Auth.GenerateResetPasswordAsync(new Employee { Email = dto.Email });
            if (!success)
            {
                Log.Warning("ForgotPassword failed, user not found: {Email}", dto.Email);
                return new ResponseDTO<bool>(404, ResponseHelper.NotFound("User"), false);
            }

            Log.Information("ForgotPassword OTP sent successfully for email: {Email}", dto.Email);
            return new ResponseDTO<bool>(200, ResponseHelper.Success("sent", "OTP"), true);
        }

        [HttpPost("VerifyOtp")]
        public async Task<ResponseDTO<string>> VerifyOtp([FromBody] VerifyOtpDTO dto)
        {
            Log.Information("VerifyOtp called for email: {Email}", dto.Email);

            var token = await _unitofwork.Auth.VerifyOtpAsync(dto);

            if (token == null)
            {
                Log.Warning("OTP verification failed for email: {Email}", dto.Email);
                return new ResponseDTO<string>(400, ResponseHelper.Invalid("Otp"), dto.Otp.ToString());
            }

            Log.Information("OTP verified successfully, token generated for email: {Email}", dto.Email);
            return new ResponseDTO<string>(200, ResponseHelper.Success("generated", "Token"), token);
        }

        [HttpPost("ResetPassword")]
        public async Task<ResponseDTO<bool>> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            Log.Information("ResetPassword called for token: {Token}", dto.Token);

            if (dto.NewPassword != dto.ConfirmPassword)
            {
                Log.Warning("ResetPassword failed, passwords do not match for token: {Token}", dto.Token);
                return new ResponseDTO<bool>(400, "Passwords do not match", false);
            }

            if (!Guid.TryParse(dto.Token, out var parsedToken))
            {
                Log.Warning("ResetPassword failed, invalid token format: {Token}", dto.Token);
                return new ResponseDTO<bool>(400, "Invalid token format", false);
            }

            var result = await _unitofwork.Auth.ResetPasswordAsync(parsedToken, dto.NewPassword);
            if (!result)
            {
                Log.Warning("ResetPassword failed, token invalid or expired: {Token}", dto.Token);
                return new ResponseDTO<bool>(404, "Token is invalid or expired", false);
            }

            Log.Information("Password reset successful for token: {Token}", dto.Token);
            return new ResponseDTO<bool>(200, "Password reset successful", true);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return false;

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
    }
}