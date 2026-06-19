using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{ 
    /// <summary>
    /// Detalle de impacto para un empleado específico dentro de una acción masiva.
    /// </summary>
    public class EmployeeMassPreviewDto
    {
        public Guid EmployeeId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        // Valores comparativos
        public string CurrentValue { get; set; } = string.Empty; // Ej: "C$ 15,000" o "Activo"
        public string NewValue { get; set; } = string.Empty;     // Ej: "C$ 16,500" o "Suspendido"

        // Estado de validación individual
        public bool IsValid { get; set; }
        public string? ValidationMessage { get; set; } // Ej: "Salario inferior al mínimo"
    }
}
