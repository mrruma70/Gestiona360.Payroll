using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Infrastructure.Persistence.Repositories
{
    public class PayrollRepository : IPayrollRepository
    {
        private readonly ApplicationDbContext _context;

        public PayrollRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PayrollHistoryData?> GetPayrollHistoryAsync(Guid employeeId, CancellationToken ct = default)
        {
            // 1. Obtener datos del empleado
            var employee = await _context.Employees
                .AsNoTracking()
                .Where(e => e.Id == employeeId)
                .Select(e => new EmployeeHeaderInfo
                {
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Code = e.Code,
                    JobPositionName = e.JobGrade != null ? e.JobGrade.JobPosition.Name : null
                })
                .FirstOrDefaultAsync(ct);

            if (employee == null)
                return null;

            // 2. Obtener historial de nóminas
            var records = await _context.PayrollRecords
                .AsNoTracking()
                .Where(r => r.EmployeeId == employeeId)
                .OrderByDescending(r => r.PayrollPeriod.EndDate)
                .Select(r => new PayrollRecordInfo
                {
                    RecordId = r.Id,
                    PeriodName = $"{r.PayrollPeriod.PayrollGroup.Name} - Período {r.PayrollPeriod.PeriodNumber}",
                    PaymentDate = r.PayrollSlip != null ? r.PayrollSlip.PaymentDate : r.PayrollPeriod.EndDate,
                    GrossIncome = r.GrossIncome,
                    INSSWorker = r.INSSWorker,
                    IR = r.IR,
                    JudicialDeductions = r.JudicialDeductions,
                    RecurringDeductionsTotal = r.RecurringDeductionsTotal,
                    NetPay = r.NetPay,
                    Status = r.Status.ToString(),
                    HasPaySlip = r.PayrollSlip != null
                })
                .ToListAsync(ct);

            return new PayrollHistoryData
            {
                EmployeeInfo = employee,
                Records = records
            };
        }
    }
}
