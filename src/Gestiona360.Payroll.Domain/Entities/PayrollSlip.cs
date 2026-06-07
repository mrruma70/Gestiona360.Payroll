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
    public class PayrollSlip : BaseEntityGuid
    {
        public Guid PayrollRecordId { get; set; }
        public DateTime PaymentDate { get; set; }

        [MaxLength(20)]
        public string PaymentMethod { get; set; }         // ACH / Cheque / Efectivo

        [MaxLength(50)]
        public string BankReference { get; set; }

        public bool ACHGenerated { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal JudicialAmount { get; set; }

        public int VacationDaysUsedInPeriod { get; set; }

        public virtual PayrollRecord PayrollRecord { get; set; }
    }
}
