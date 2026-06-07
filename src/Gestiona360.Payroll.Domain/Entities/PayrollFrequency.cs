using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Common;

namespace Gestiona360.Payroll.Domain.Entities
{
    public class PayrollFrequency : BaseEntityInt
    {
        [Required, MaxLength(20)]
        public string Name { get; set; }          // Mensual / Quincenal / Semanal

        [Required, MaxLength(20)]
        public string Code { get; set; }          // MONTHLY / BIWEEKLY / WEEKLY

        public int DaysPerPeriod { get; set; }
        public int PeriodsPerYear { get; set; }
        [MaxLength(200)]
        public string Description { get; set; }
    }
}
