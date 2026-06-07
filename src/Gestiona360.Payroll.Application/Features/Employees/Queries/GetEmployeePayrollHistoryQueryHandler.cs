using Gestiona360.Payroll.Application.Contracts.DTOs.Employees;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.Employees.Queries
{

    public class GetEmployeePayrollHistoryQueryHandler : IRequestHandler<GetEmployeePayrollHistoryQuery, EmployeePayrollHistoryResultDto>
    {
        private readonly ApplicationDbContext _context;

        public GetEmployeePayrollHistoryQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EmployeePayrollHistoryResultDto> Handle(GetEmployeePayrollHistoryQuery request, CancellationToken cancellationToken)
        {
            // 1. Obtener datos básicos del empleado para el encabezado
            var employee = await _context.Employees
                .Include(e => e.JobGrade).ThenInclude(jg => jg!.JobPosition)
                .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken);

            if (employee == null)
                throw new KeyNotFoundException("Empleado no encontrado.");

            // 2. Obtener el historial de nóminas, ordenado del más reciente al más antiguo
            var records = await _context.PayrollRecords
                .Where(r => r.EmployeeId == request.EmployeeId)
                .Include(r => r.PayrollPeriod)
                .Include(r => r.PayrollSlip) // Para saber si ya se generó la colilla
                .OrderByDescending(r => r.PayrollPeriod.EndDate)
                .Select(r => new PayrollHistoryRecordDto
                {
                    RecordId = r.Id,
                    PeriodName = $"{r.PayrollPeriod.PayrollGroup.Name} - Período {r.PayrollPeriod.PeriodNumber}",
                    PaymentDate = r.PayrollSlip != null ? r.PayrollSlip.PaymentDate : r.PayrollPeriod.EndDate,
                    GrossIncome = r.GrossIncome,
                    // Calculamos las deducciones totales en el backend para no saturar el frontend
                    TotalDeductions = r.INSSWorker + r.IR + r.JudicialDeductions + r.RecurringDeductionsTotal,
                    NetPay = r.NetPay,
                    Status = r.Status,
                    HasPaySlip = r.PayrollSlip != null
                })
                .ToListAsync(cancellationToken);

            // 3. Calcular resumen YTD (Year to Date) del año actual
            var currentYear = DateTime.UtcNow.Year;
            var ytdRecords = records.Where(r => r.PaymentDate.Year == currentYear).ToList();

            return new EmployeePayrollHistoryResultDto
            {
                EmployeeName = $"{employee.FirstName} {employee.LastName}",
                EmployeeCode = employee.Code,
                CurrentPosition = employee.JobGrade?.JobPosition?.Name ?? "Sin puesto",
                Records = records,
                TotalYtdGross = ytdRecords.Sum(r => r.GrossIncome),
                TotalYtdNet = ytdRecords.Sum(r => r.NetPay)
            };
        }
    }
}
