using EmpManage.Interfaces;
using EmpManage.Repositories;

namespace EmpManage.Extensions
{
    public static class ServiceRegister
    {
        public static IServiceCollection AddRepositories (this IServiceCollection services)
        {
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IAddPermissionRepository, AddPermissionRepository>();
            services.AddScoped<IEmailRepository, EmailRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
