using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Common;

namespace Gestiona360.Payroll.Domain.Entities
{
    public class HolidayCalendar : BaseEntityInt
    {
        public int Year { get; set; }
        public DateTime Date { get; set; }

        [Required, MaxLength(100)]
        public string Description { get; set; }

        public bool IsPaidDouble { get; set; } = true;   // recargo 100%
        public DateTime? SubstituteDate { get; set; }    // si cae domingo
        public bool IsNewLaw1272 { get; set; }           // feriado agregado en 2026
    }
}
