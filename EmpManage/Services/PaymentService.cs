using EmpManage.Interfaces;

namespace EmpManage.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<string> CreatePaymentIntentAsync(long amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount should be greater than zero");

            return await _paymentRepository.CreatePaymentIntentAsync(amount);
        }
    }
}
