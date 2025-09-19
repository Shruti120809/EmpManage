namespace EmpManage.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAuthRepository Auth { get; }
        IEmployeeRepository Employee { get; }
        IAddPermissionRepository AddPermission { get; }
        IMenuRepository Menu { get; }
        IEmailRepository Email { get; }
        IPaymentRepository PaymentRepository { get; }

        Task<int> CompleteAsync();
    }
}
