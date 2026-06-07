using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs
{
    public class WithholdingDto
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;

        public string EmployeeCode { get; set; } = string.Empty;
        public decimal GrossSalary { get; set; }
        public decimal INSSWorker { get; set; }
        public decimal TaxableIncome { get; set; }
        public decimal IRWithheld { get; set; }
    }
}
