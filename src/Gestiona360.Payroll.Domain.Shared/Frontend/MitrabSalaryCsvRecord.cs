using System;
using System.Collections.Generic;
using System.Text;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    /// <summary>
    /// DTO para mapeo de registros CSV de salarios mínimos MITRAB.
    /// </summary>
    public class MitrabSalaryCsvRecord
    {
        public int Year { get; set; }
        public string SectorName { get; set; } = string.Empty;
        public decimal MonthlyAmountNIO { get; set; }
        public decimal MonthlyAmountUSD { get; set; }
        public string LegalReference { get; set; } = string.Empty;
    }
}
