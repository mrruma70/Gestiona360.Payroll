using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gestiona360.Payroll.Domain.Common;

namespace Gestiona360.Payroll.Domain.Entities
{
    public class EmployeeConceptSetting : BaseEntityGuid
    {
        public Guid EmployeeId { get; set; }
        public Guid PayrollConceptId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; } = true;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? CustomValue { get; set; }          // Monto fijo o porcentaje personalizado

        // Para amortizaciones (préstamos)
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrincipal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal RemainingBalance { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal InstallmentAmount { get; set; }

        public int InstallmentTotal { get; set; }          // total de cuotas
        public int InstallmentCurrent { get; set; }        // cuota actual

        public bool IsRecurrent { get; set; }
        [MaxLength(20)]
        public string ApplicationPeriodicity { get; set; } // Monthly / Biweekly / Weekly

        public bool AutoReprogram { get; set; }
        public string LastReprogramReason { get; set; }
        public Guid? LinkedToPersonalActionId { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual PayrollConcept PayrollConcept { get; set; }
        public virtual ICollection<PayrollConceptTransaction> Transactions { get; set; }
        public virtual ICollection<AmortizationLedger> AmortizationLedgers { get; set; }
    }
}
