using System;
using System.Collections.Generic;
using System.Text;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    /// <summary>
    /// Datos de historial de nómina para un empleado.
    /// </summary>
    public class PayrollHistoryData
    {
        public EmployeeHeaderInfo EmployeeInfo { get; set; } = null!;
        public List<PayrollRecordInfo> Records { get; set; } = new();
    }

    public class EmployeeHeaderInfo
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? JobPositionName { get; set; }
    }

    public class PayrollRecordInfo
    {
        public Guid RecordId { get; set; }
        public string PeriodName { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public decimal GrossIncome { get; set; }
        public decimal INSSWorker { get; set; }
        public decimal IR { get; set; }
        public decimal JudicialDeductions { get; set; }
        public decimal RecurringDeductionsTotal { get; set; }
        public decimal NetPay { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool HasPaySlip { get; set; }
    }
}
