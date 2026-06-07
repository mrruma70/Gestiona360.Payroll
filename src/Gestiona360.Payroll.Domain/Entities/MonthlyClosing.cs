using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Common;

namespace Gestiona360.Payroll.Domain.Entities
{
    public class MonthlyClosing : BaseEntityGuid
    {
        public Guid PayrollPeriodId { get; set; }
        [MaxLength(20)]
        public string Status { get; set; } = "Closed";   // Closed / Audited
        public Guid ExecutedBy { get; set; }
        [MaxLength(128)]
        public string HashSHA256 { get; set; }

        public virtual PayrollPeriod PayrollPeriod { get; set; }
    }
}
