using FluentValidation;
using Gestiona360.Payroll.Application.Contracts.Reports;
using Gestiona360.Payroll.Application.Features.Employees.Exports;
using Gestiona360.Payroll.Infrastructure.Reporting;
using Gestiona360.Payroll.Infrastructure.Reporting.Renderers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Gestiona360.Payroll.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // MediatR - escanea todos los handlers en el assembly actual
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

            // Pipeline behavior de validación
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            // ============================================
            // MOTOR DE REPORTES
            // ============================================
            services.AddScoped<IReportEngine, ReportEngineService>();

            // Renderizadores (se inyectan como IEnumerable<IReportRenderer>)
            services.AddScoped<IReportRenderer, ExcelReportRenderer>();
            services.AddScoped<IReportRenderer, CsvReportRenderer>();
            services.AddScoped<IReportRenderer, PdfReportRenderer>();
            services.AddScoped<IReportRenderer, XmlReportRenderer>();
            services.AddScoped<IReportRenderer, PdfFichaRenderer>();
            services.AddScoped<EmployeeExportService>();

            // Escanea todos los validadores (AbstractValidator<T>) en este assembly
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

            return services;
        }
    }
}
