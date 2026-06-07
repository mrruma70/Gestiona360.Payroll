using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.Employees
{
    public class EmployeePayrollHistoryResultDto
    {
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public string CurrentPosition { get; set; } = string.Empty;
        public List<PayrollHistoryRecordDto> Records { get; set; } = new();

        // Resumen anual (opcional pero muy útil)
        public decimal TotalYtdGross { get; set; }
        public decimal TotalYtdNet { get; set; }
    }
}
