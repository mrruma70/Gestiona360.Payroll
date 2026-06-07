using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Common;

namespace Gestiona360.Payroll.Domain.Entities
{
    public class PayrollRecord : BaseEntityGuid
    {
        public Guid PayrollPeriodId { get; set; }
        public Guid EmployeeId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GrossIncome { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal INSSWorker { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal INSSEmployer { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal INATEC { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal IR { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal JudicialDeductions { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal RecurringDeductionsTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal VacationPayAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal NetPay { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Draft";     // Draft / Closed

        public virtual PayrollPeriod PayrollPeriod { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual ICollection<PayrollConceptTransaction> Transactions { get; set; }
        public virtual PayrollSlip PayrollSlip { get; set; }
    }
}
