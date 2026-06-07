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
    public class PayrollConcept : BaseEntityGuid
    {
        [Required, MaxLength(20)]
        public string Code { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, MaxLength(20)]
        public string Type { get; set; }                 // Perception / Deduction

        [MaxLength(20)]
        public string Category { get; set; }             // Legal / Judicial / Contractual / Occasional

        public int Priority { get; set; }                // 1..4 (para orden de deducción)
        public bool ApplyToINSS { get; set; }
        public bool ApplyToIR { get; set; }
        public bool ApplyToINATEC { get; set; }

        [Required, MaxLength(30)]
        public string CalculationMethod { get; set; }    // FixedAmount / PercentageOfGross / PercentageOfNet / PerDay / Formula

        [Column(TypeName = "decimal(18,4)")]
        public decimal CalculationValue { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MaxAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinAmount { get; set; }

        public bool IsRecurrentByDefault { get; set; }
        public bool RequiresApproval { get; set; }
        public bool CanBeReprogrammed { get; set; }
        public bool ShowInPaySlipAsDetail { get; set; } = true;

        [MaxLength(50)]
        public string LegalReference { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
