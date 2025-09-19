using EmpManage.DTOs;
using EmpManage.Helper;
using EmpManage.Interfaces;
using EmpManage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace EmpManage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IUnitOfWork _unitofwork;

        public PaymentController(IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;
        }

        [HttpPost("CreatePayment")]
        public async Task<ResponseDTO<string>> CreatePaymentIntent([FromBody] PaymentRequest request)
        {
            var clientSecret = await _unitofwork.PaymentRepository.CreatePaymentIntentAsync(request.Amount);
            return new ResponseDTO<string>(
                    200,
                    ResponseHelper.Success("fetched", "Data"),
                    clientSecret);
        }
    }
}
