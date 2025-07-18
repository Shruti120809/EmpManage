namespace EmpManage.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAuthRepository Auth { get; }
        IEmployeeRepository Employee { get; }
        IAddPermissionRepository AddPermission { get; }

        Task<int> CompleteAsync();
    }
}
