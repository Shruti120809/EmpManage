using EmpManage.Data;
using EmpManage.Interfaces;

namespace EmpManage.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;


        public IAuthRepository Auth { get; }
        public IEmployeeRepository Employee { get; }
        public IMenuRepository Menu { get; }
        public IAddPermissionRepository AddPermission { get; }
        public IEmailRepository Email { get; }
        public IPaymentRepository PaymentRepository { get; }

        public UnitOfWork(AppDbContext context,
                          IAuthRepository authRepository,
                          IEmployeeRepository employeeRepository, 
                          IMenuRepository menuRepository,
                          IAddPermissionRepository addPermissionRepository,
                          IEmailRepository emailRepository,
                          IPaymentRepository paymentRepository)
                        
        {
            _context = context;
            Auth = authRepository;
            Employee = employeeRepository;
            Menu = menuRepository;
            AddPermission = addPermissionRepository;
            Email = emailRepository;
            PaymentRepository = paymentRepository;
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
