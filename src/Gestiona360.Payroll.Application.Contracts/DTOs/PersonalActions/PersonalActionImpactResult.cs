using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.PersonalActions
{
    public class PersonalActionImpactResult
    {
        public bool IsRetroactive { get; set; }
        public bool IsFutureAction { get; set; }
        public string AffectedPayrollPeriodName { get; set; } = string.Empty;
        public string PeriodStatus { get; set; } = string.Empty; // Open, Closed, Paid
        public bool RequiresApproval { get; set; }
        public string UserMessage { get; set; } = string.Empty;
        public decimal? EstimatedFinancialImpact { get; set; }
    }
}
