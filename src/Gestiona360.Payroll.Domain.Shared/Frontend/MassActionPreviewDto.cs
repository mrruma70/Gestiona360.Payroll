using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    /// <summary>
    /// Resultado de la simulación de una acción masiva antes de ejecutarla.
    /// </summary>
    public class MassActionPreviewDto
    {
        public string BatchReference { get; set; } = string.Empty;
        public int TotalEmployees { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public string CausalDescription { get; set; } = string.Empty;
        public DateTime EffectiveDate { get; set; }
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Lista detallada de cómo afectará a cada empleado.
        /// </summary>
        public List<EmployeeMassPreviewDto> EmployeesPreview { get; set; } = new();

        /// <summary>
        /// Errores globales de validación (ej. "El período está cerrado").
        /// </summary>
        public List<string> ValidationErrors { get; set; } = new();

        public bool IsValid => !ValidationErrors.Any() && EmployeesPreview.All(e => e.IsValid);
    }
}
