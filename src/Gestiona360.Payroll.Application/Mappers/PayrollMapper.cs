using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Application.Contracts.DTOs.Employees;
using Gestiona360.Payroll.Domain.Shared.Frontend;

namespace Gestiona360.Payroll.Application.Mappers
{
    public static class PayrollMapper
    {
        public static EmployeePayrollHistoryResultDto ToHistoryResultDto(PayrollHistoryData data)
        {
            var currentYear = DateTime.UtcNow.Year;
            var ytdRecords = data.Records.Where(r => r.PaymentDate.Year == currentYear).ToList();

            return new EmployeePayrollHistoryResultDto
            {
                EmployeeName = $"{data.EmployeeInfo.FirstName} {data.EmployeeInfo.LastName}",
                EmployeeCode = data.EmployeeInfo.Code,
                CurrentPosition = data.EmployeeInfo.JobPositionName ?? "Sin puesto",
                Records = data.Records.Select(ToPayrollHistoryRecordDto).ToList(),
                TotalYtdGross = ytdRecords.Sum(r => r.GrossIncome),
                TotalYtdNet = ytdRecords.Sum(r => r.NetPay)
            };
        }

        private static PayrollHistoryRecordDto ToPayrollHistoryRecordDto(PayrollRecordInfo r)
        {
            return new PayrollHistoryRecordDto
            {
                RecordId = r.RecordId,
                PeriodName = r.PeriodName,
                PaymentDate = r.PaymentDate,
                GrossIncome = r.GrossIncome,
                TotalDeductions = r.INSSWorker + r.IR + r.JudicialDeductions + r.RecurringDeductionsTotal,
                NetPay = r.NetPay,
                Status = r.Status,
                HasPaySlip = r.HasPaySlip
            };
        }
    }
}
