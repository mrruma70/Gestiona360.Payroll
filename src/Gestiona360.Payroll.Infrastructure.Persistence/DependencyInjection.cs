using Gestiona360.Payroll.Application.Abstractions.Repositories;
using Gestiona360.Payroll.Domain.Services;
using Gestiona360.Payroll.Infrastructure.Persistence.Repositories;
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
            // ✅ REGISTRO EXPLÍCITO DEL SERVICIO DE DOMINIO
        
            //services.AddScoped<IPayrollFrequencyRepository, PayrollFrequencyRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();


            // 3. Registrar Repositorios automáticamente con Scrutor
            services.Scan(scan => scan
               .FromAssemblyOf<UnitOfWork>()
                .AddClasses(classes => classes.Where(c => c.Name.EndsWith("Repository")))
                .AsMatchingInterface()
                .WithScopedLifetime());


            // Debe ir DESPUÉS del registro de repositorios con Scrutor
            services.AddScoped<EmployeeDomainService>();
            services.AddScoped<BranchDomainService>();
            services.AddScoped<CompanyDomainService>();
            services.AddScoped<TaxScheduleDomainService>();
            services.AddScoped<MinimumWageScheduleDomainService>();
            services.AddScoped<InatecConfigDomainService>();
            services.AddScoped<InssConfigDomainService>();
            services.AddScoped<PersonalActionDomainService>();
            services.AddScoped<MassActionPreviewDomainService>();

            return services;
        }
    }
}

