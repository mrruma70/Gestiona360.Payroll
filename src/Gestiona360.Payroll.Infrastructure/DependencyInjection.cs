using Gestiona360.Payroll.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Gestiona360.Payroll.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Regla de oro: Interfaces del Domain apuntan a Implementaciones de Infrastructure
        //services.AddScoped<IUnitOfWork, UnitOfWork>();
        //services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        //services.AddScoped<IPayrollRepository, PayrollRepository>();

        return services;
    }
}
