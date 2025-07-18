using EmpManage.Data;
using EmpManage.Interfaces;

namespace EmpManage.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;


        public IAuthRepository Auth { get; }
        public IEmployeeRepository Employee { get; }
        public IAddPermissionRepository AddPermission { get; }

        public UnitOfWork(AppDbContext context,
                          IAuthRepository authRepository,
                          IEmployeeRepository employeeRepository, 
                          IAddPermissionRepository addPermissionRepository)
        {
            _context = context;
            Auth = authRepository;
            Employee = employeeRepository;
            AddPermission = addPermissionRepository; 
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
