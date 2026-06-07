using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Common;

namespace Gestiona360.Payroll.Domain.Entities
{
    public class INATECRecord : BaseEntityGuid
    {
        public Guid PayrollPeriodId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPayrollGross { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal Rate { get; set; } = 2.0m;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime RecordDate { get; set; } = DateTime.UtcNow;

        public virtual PayrollPeriod PayrollPeriod { get; set; }
    }
}
