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
    public class INSSConfig : BaseEntityInt
    {


        // CAMPOS SAGRADOS DE VIGENCIA TEMPORAL
        [Required]
        public DateOnly EffectiveFrom { get; set; }

        public DateOnly? EffectiveTo { get; set; } // NULL = Vigente actualmente

        [MaxLength(150)]
        public string LegalReference { get; set; } = string.Empty; // Ej: "Convenio INSS 2026"

        // DATOS DE CONFIGURACIÓN
        [Required]
        public decimal RateWorker { get; set; }

        [Required]
        public decimal RateEmployerSmall { get; set; }

        [Required]
        public decimal RateEmployerLarge { get; set; }

        [Required]
        public decimal MaxSalaryCap { get; set; }

 

        // PROPIEDAD DERIVADA (solo para mostrar en UI)
        public int Year => EffectiveFrom.Year;


    }
}
