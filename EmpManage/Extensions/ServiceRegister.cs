using EmpManage.Helper;
using EmpManage.Interfaces;
using EmpManage.Repositories;
using EmpManage.Services;

namespace EmpManage.Extensions
{
    public static class ServiceRegister
    {
        public static IServiceCollection AddRepositories (this IServiceCollection services, IConfiguration config)
        {
            var stripeSecretKey = config["Stripe:SecretKey"];

            //UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //Repositorys
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IAddPermissionRepository, AddPermissionRepository>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IEmailRepository, EmailRepository>();
            services.AddScoped<IPaymentRepository>(sp => new PaymentRepository(stripeSecretKey));

            //Services
            services.AddScoped<ICreateTokenService, CreateTokenService>();
            services.AddScoped<IPaymentService, PaymentService>();

            return services;
        }
    }
}
