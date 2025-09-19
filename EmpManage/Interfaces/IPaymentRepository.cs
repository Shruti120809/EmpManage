namespace EmpManage.Interfaces
{
    public interface IPaymentRepository
    {
        Task<string> CreatePaymentIntentAsync(long amount);
    }
}
