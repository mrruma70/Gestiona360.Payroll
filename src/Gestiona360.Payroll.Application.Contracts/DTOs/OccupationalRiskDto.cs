using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs
{
    /// <summary>
    /// DTO para OccupationalRisk
    /// Ejemplo: R01 (Bajo), R02 (Medio), R03 (Alto)
    /// </summary>
    public class OccupationalRiskDto : CatalogItemDto
    {
        [Required]
        [Range(0, 100, ErrorMessage = "La tasa debe estar entre 0 y 100")]
        public decimal INSSRiskRate { get; set; }

        public string LegalReference { get; set; }
    }

}
