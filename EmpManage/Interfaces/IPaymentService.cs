namespace EmpManage.Interfaces
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentIntentAsync(long amount);
    }
}
