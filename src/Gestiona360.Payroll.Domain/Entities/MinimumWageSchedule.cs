using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Common;

namespace Gestiona360.Payroll.Domain.Entities
{
    public class MinimumWageSchedule : BaseEntityInt
    {
   

        [Required]
        public DateOnly EffectiveFrom { get; set; }

        public DateOnly? EffectiveTo { get; set; }

        [Required, MaxLength(150)]
        public string LegalReference { get; set; } = string.Empty; // Ej: "Acuerdo MITRAB 015-2024"


        public int Year => EffectiveFrom.Year;

        // RELACIÓN 1:N CON EL DETALLE
        public virtual ICollection<MinimumWage> Wages { get; set; } = new List<MinimumWage>();
    }
}
