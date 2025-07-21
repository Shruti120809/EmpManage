namespace EmpManage.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAuthRepository Auth { get; }
        IEmployeeRepository Employee { get; }
        IAddPermissionRepository AddPermission { get; }
        IEmailRepository Email { get; }

        Task<int> CompleteAsync();
    }
}
