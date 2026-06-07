using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs
{
    /// <summary>
    /// DTO para IR_TaxBracket
    /// Tabla progresiva del Impuesto sobre la Renta
    /// </summary>
    public class IR_TaxBracketDto : CatalogItemDto
    {
        public int Id { get; set; }


        [Required]
        public int Year { get; set; } = DateTime.Now.Year;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal FromAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? ToAmount { get; set; } // null = infinito

        [Required]
        [Range(0, double.MaxValue)]
        public decimal FixedTax { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal MarginalRate { get; set; }

        public int ScheduleId { get; set; }
    }

}
