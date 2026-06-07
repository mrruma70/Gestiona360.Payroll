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
    public class AmortizationLedger : BaseEntityGuid
    {
        public Guid EmployeeConceptSettingId { get; set; }
        public Guid? PayrollConceptTransactionId { get; set; }

        public DateTime TransactionDate { get; set; }

        [Required, MaxLength(30)]
        public string TransactionType { get; set; }       // PrincipalIssuance / Installment / Reprogram / WriteOff / BalanceAdjustment

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BalanceBefore { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BalanceAfter { get; set; }

        public string Notes { get; set; }

        public virtual EmployeeConceptSetting EmployeeConceptSetting { get; set; }
        public virtual PayrollConceptTransaction PayrollConceptTransaction { get; set; }
    }
}
