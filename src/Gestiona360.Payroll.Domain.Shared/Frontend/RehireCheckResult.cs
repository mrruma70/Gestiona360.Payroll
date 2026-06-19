using System;
using System.Collections.Generic;
using System.Text;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    public class RehireCheckResult
    {
        public bool IsRehire { get; set; }
        public string? PreviousEmployeeName { get; set; }
        public string? PreviousEmployeeCode { get; set; }
        public DateTime? TerminationDate { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
