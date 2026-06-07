using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.Employees
{
    public class PayrollHistoryRecordDto
    {
        public Guid RecordId { get; set; }
        public string PeriodName { get; set; } = string.Empty; // Ej: "Enero 2026 - Quincena 1"
        public DateTime PaymentDate { get; set; }
        public decimal GrossIncome { get; set; }
        public decimal TotalDeductions { get; set; } // INSS + IR + Judiciales + Recurrentes
        public decimal NetPay { get; set; }
        public string Status { get; set; } = string.Empty; // "Closed", "Audited", etc.
        public bool HasPaySlip { get; set; }
    }
}
