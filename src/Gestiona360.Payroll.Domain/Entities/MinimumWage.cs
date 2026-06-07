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
    public class MinimumWage : BaseEntityInt
    {
     
        [Required]
        public int ScheduleId { get; set; }

        [ForeignKey(nameof(ScheduleId))]
        public virtual MinimumWageSchedule Schedule { get; set; } = null!;

        [Required, MaxLength(100)]
        public string Sector { get; set; } = string.Empty;

        [Required]
        public decimal MonthlyAmountNIO { get; set; }

        [Required]
        public decimal MonthlyAmountUSD { get; set; }

    }
}
