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
    public class IR_TaxBracket : BaseEntityInt
    {
        [Required]
        public int ScheduleId { get; set; }

        [ForeignKey(nameof(ScheduleId))]
        public virtual IrTaxSchedule Schedule { get; set; } = null!;

        [Required]
        public decimal FromAmount { get; set; }

        public decimal? ToAmount { get; set; }

        [Required]
        public decimal FixedTax { get; set; }

        [Required]
        public decimal MarginalRate { get; set; }
    }
}
