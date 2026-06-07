using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Common;

namespace Gestiona360.Payroll.Domain.Entities
{
    public class ContractType : BaseEntityInt
    {
        [Required, MaxLength(50)]
        public string Name { get; set; }               // Indefinido, Plazo fijo, Temporal

        [Required, MaxLength(20)]
        public string DurationType { get; set; }       // Indefinido / PlazoFijo / Temporal

        [Required, MaxLength(20)]
        public string WorkdayType { get; set; }        // Completa / Parcial / PorHoras

        [Required, MaxLength(20)]
        public string SalaryCalcType { get; set; }     // MensualFijo / Quincenal / Semanal / PorHora

        public bool IncludesServices { get; set; }     // true para honorarios
        public bool ApplyINSS { get; set; } = true;
        public bool ApplyIR { get; set; } = true;
        public bool ApplyINATEC { get; set; } = true;
        public int ApplyVacationsDays { get; set; } = 30;  // 30 días por año
        public bool ApplyThirteenthMonth { get; set; } = true;
        public bool ApplyIndemnity { get; set; } = true;
        public int NoticeDays { get; set; } = 15;          // preaviso
        public string OvertimeRules { get; set; }          // JSON con reglas específicas
        public string DefaultPayComponents { get; set; }   // JSON con conceptos por defecto

        // Nuevos campos para reglas adicionales
        [Range(0, 90)]
        public int ProbationDays { get; set; }   // 0 = sin período de prueba, 30 o 90 según ley
        public bool AllowsBenefitsInKind { get; set; }
        public bool RequiresWorkPermit { get; set; }
        public bool IsTrustPosition { get; set; }  // Si el puesto por defecto es de confianza

        public bool HasProbationPeriod { get; set; } // Campo calculado para facilitar consultas (true si ProbationDays > 0)
    }
}
