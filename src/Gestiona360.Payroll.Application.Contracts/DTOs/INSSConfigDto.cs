using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs
{
    /// <summary>
    /// DTO para INSSConfig
    /// Configuración de tasas del INSS por año
    /// </summary>
    public class INSSConfigDto : CatalogItemDto
    {
        [Required]
        public int Year { get; set; } = DateTime.Now.Year;

        [Required]
        [Range(0, 100)]
        public decimal RateWorker { get; set; } = 7.0m;

        [Required]
        [Range(0, 100)]
        public decimal RateEmployerSmall { get; set; } = 21.5m;

        [Required]
        [Range(0, 100)]
        public decimal RateEmployerLarge { get; set; } = 22.5m;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal MaxSalaryCap { get; set; }

        public string LegalReference { get; set; } = string.Empty;

        public DateOnly EffectiveFrom { get; set; }
        public DateOnly? EffectiveTo { get; set; }
    }

}
