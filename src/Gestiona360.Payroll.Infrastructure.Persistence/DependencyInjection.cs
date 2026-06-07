using System.Reflection;
using Gestiona360.Payroll.Application.Abstractions.Repositories;
using Gestiona360.Payroll.Domain.Entities; // Donde reside ApplicationUser
using Gestiona360.Payroll.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gestiona360.Payroll.Infrastructure.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Registrar DbContext (debe heredar de IdentityDbContext)
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            // Registrar repositorios y UnitOfWork
            services.AddScoped<IPayrollFrequencyRepository, PayrollFrequencyRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

               


            // Si tienes otros repositorios, regístralos aquí

            return services;
        }
    }
}

