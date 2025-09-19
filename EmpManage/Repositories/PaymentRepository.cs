using EmpManage.Interfaces;
using Microsoft.Identity.Client;
using Stripe;

namespace EmpManage.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        public PaymentRepository(string SecretKey) 
        {
            StripeConfiguration.ApiKey = SecretKey;
        }
        public async Task<string> CreatePaymentIntentAsync(long amount)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = amount,
                Currency = "sgd",
                PaymentMethodTypes = new List<string> { "card" }
            };

            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(options);
            return intent.ClientSecret;
        }
    }
}
