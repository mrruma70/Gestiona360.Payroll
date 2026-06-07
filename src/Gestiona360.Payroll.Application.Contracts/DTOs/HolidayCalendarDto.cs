using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs
{
    /// <summary>
    /// DTO para HolidayCalendar
    /// Días feriados obligatorios
    /// </summary>
    public class HolidayCalendarDto : CatalogItemDto
    {
        [Required]
        public int Year { get; set; } = DateTime.Now.Year;

        [Required]
        public DateTime Date { get; set; }

        public bool IsPaidDouble { get; set; } = true;

        public DateTime? SubstituteDate { get; set; }

        public bool IsNewLaw1272 { get; set; }
    }

}
