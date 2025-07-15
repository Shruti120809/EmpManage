namespace EmpManage.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAuthRepository Auth { get; }
        IEmployeeRepository Employee { get; }

        Task<int> CompleteAsync();
    }
}
