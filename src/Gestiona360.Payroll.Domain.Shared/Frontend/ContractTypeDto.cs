using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    /// <summary>
    /// DTO para ContractType
    /// Tipos de contratos laborales
    /// </summary>
    public class ContractTypeDto : CatalogItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;


        [Required]
        [StringLength(20)]
        public string DurationType { get; set; } // Indefinido / PlazoFijo / Temporal

        [Required]
        [StringLength(20)]
        public string WorkdayType { get; set; } // Completa / Parcial / PorHoras

        [Required]
        [StringLength(20)]
        public string SalaryCalcType { get; set; } // MensualFijo / Quincenal / etc.

        public bool ApplyINSS { get; set; } = true;
        public bool ApplyIR { get; set; } = true;
        public bool ApplyINATEC { get; set; } = true;
        public bool ApplyVacations { get; set; } = true;
        public bool ApplyThirteenthMonth { get; set; } = true;
        public bool ApplyIndemnity { get; set; } = true;
        public int ApplyVacationsDays { get; set; }

        [Range(1, 365)]
        public int NoticeDays { get; set; } = 15;
    }

}
