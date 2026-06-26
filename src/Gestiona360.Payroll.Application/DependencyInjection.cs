using FluentValidation;
using Gestiona360.Payroll.Application.Contracts.Reports;
using Gestiona360.Payroll.Application.Contracts.Services;
using Gestiona360.Payroll.Application.Features.PersonalActions.Commands.ExecutePersonalAction;
using Gestiona360.Payroll.Application.Features.PersonalActions.Strategies;
using Gestiona360.Payroll.Application.Pipeline;
using Gestiona360.Payroll.Application.Services;
using Gestiona360.Payroll.Domain.Interfaces;
using Gestiona360.Payroll.Domain.Services;
using Gestiona360.Payroll.Infrastructure.Persistence.Repositories;
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
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehavior<,>));


            // Registro automático de todas las estrategias de acciones
            // Esto reemplaza el servicio monolítico PersonalActionExecutionService
            services.Scan(scan => scan
             .FromAssemblyOf<ExecutePersonalActionCommandHandler>()
             .AddClasses(classes => classes
                 .AssignableToAny(typeof(IPersonalActionStrategy), typeof(IPayrollService))
             )
             .AsImplementedInterfaces()
             .WithScopedLifetime());

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
            services.AddScoped<IExcelGenerator, ClosedXmlExcelGenerator>();
            services.AddScoped<IEmployeeExportService, EmployeeExportService>();
         

            services.AddScoped<IPayrollService, PayrollPeriodService>();
            //services.AddScoped<IPersonalActionRepository, PersonalActionRepository>();
            services.AddScoped<IPayrollRepository, PayrollRepository>();
            services.AddScoped<EmployeeBarcodeService>();
        
            services.AddScoped<ITaxScheduleRepository, TaxScheduleRepository>();
            services.AddScoped<ICsvImportService, CsvImportService>();
            services.AddScoped<IMinimumWageScheduleRepository, MinimumWageScheduleRepository>();
            services.AddScoped<IInatecConfigRepository, InatecConfigRepository>();
            services.AddScoped<IInssConfigRepository, InssConfigRepository>();
            services.AddScoped<IPayrollGroupRepository, PayrollGroupRepository>();
            services.AddScoped<IPersonalActionRepository, PersonalActionRepository>();
            services.AddScoped<IPayrollPeriodRepository, PayrollPeriodRepository>();
            services.AddScoped<IEmployeeShiftAssignmentRepository, EmployeeShiftAssignmentRepository>();
            services.AddScoped<IPersonalActionValidationService, PersonalActionValidationService>();
            services.AddScoped<IMinimumWageRepository, MinimumWageRepository>();


            // Escanea todos los validadores (AbstractValidator<T>) en este assembly
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

            return services;
        }
    }
}
