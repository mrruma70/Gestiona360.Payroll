using Gestiona360.Payroll.Application.Contracts;
using Gestiona360.Payroll.Domain.Entities; // Donde reside ApplicationUser
using Gestiona360.Payroll.Infrastructure.Identity.Services;
using Gestiona360.Payroll.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gestiona360.Payroll.Infrastructure.Identity
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddIdentityInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // El DbContext ya está registrado en Persistence, pero aquí lo usamos
            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


            // Registrar la interfaz
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

            services.AddScoped<ITokenGenerationService, TokenGenerationService>();

            return services;
        }
    }
}

