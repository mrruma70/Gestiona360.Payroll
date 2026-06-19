using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    /// <summary>
    /// DTO para MinimumWage
    /// Salarios mínimos por sector y año
    /// </summary>
    public class MinimumWageDto : CatalogItemDto
    {
        [Required]
        public int Year { get; set; } = DateTime.Now.Year;

        [Required]
        [StringLength(50)]
        public string Sector { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal MonthlyAmountNIO { get; set; }

        [Range(0, double.MaxValue)]
        public decimal MonthlyAmountUSD { get; set; }

        public int ScheduleId { get; set; }
    }

}
