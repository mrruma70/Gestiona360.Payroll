using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.PersonalActions
{
    /// <summary>
    /// DTO para crear una Acción de Personal masiva (afecta a N empleados).
    /// Genera un registro individual por cada empleado, todos vinculados por BatchReference.
    /// </summary>
    public class CreateMassPersonalActionDto
    {
        // ═══════════════════════════════════════════════════════════════
        // DATOS OBLIGATORIOS DEL LOTE
        // ═══════════════════════════════════════════════════════════════

        [Required(ErrorMessage = "La referencia del lote es obligatoria")]
        [MaxLength(100, ErrorMessage = "La referencia del lote no puede exceder 100 caracteres")]
        [RegularExpression(@"^[A-Z0-9\-]+$", ErrorMessage = "Solo se permiten mayúsculas, números y guiones")]
        public string BatchReference { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de acción es obligatorio")]
        public string ActionType { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de efecto es obligatoria")]
        public DateTime EffectiveDate { get; set; }

        /// <summary>Fecha de fin (obligatorio para suspensiones)</summary>
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "La justificación es obligatoria")]
        [MinLength(20, ErrorMessage = "La justificación debe tener al menos 20 caracteres")]
        [MaxLength(2000)]
        public string Justification { get; set; } = string.Empty;

        // ═══════════════════════════════════════════════════════════════
        // CAUSAL Y AUTORIZACIONES LEGALES
        // ═══════════════════════════════════════════════════════════════

        [Required(ErrorMessage = "La causal legal es obligatoria")]
        [MaxLength(200)]
        public string CausalDescription { get; set; } = string.Empty;

        /// <summary>Tipo de suspensión (requerido si es MassSuspension)</summary>
        public string? SuspensionType { get; set; }

        /// <summary>Tipo de terminación (requerido si es MassTermination)</summary>
        public string? TerminationType { get; set; }

        /// <summary>N° de autorización MITRAB (obligatorio para MassForceMajeure)</summary>
        [MaxLength(100)]
        public string? MitrabAuthorizationNumber { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // REGLA DE CÁLCULO (para SalaryChange masivo)
        // ═══════════════════════════════════════════════════════════════

        /// <summary>Tipo de regla: "Fixed" (monto fijo) o "Percentage" (porcentaje)</summary>
        public string? RuleType { get; set; }

        /// <summary>Valor de la regla (monto fijo o porcentaje)</summary>
        [Range(0, 10000000)]
        public decimal? RuleValue { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // EMPLEADOS AFECTADOS
        // ═══════════════════════════════════════════════════════════════

        [Required(ErrorMessage = "Debe seleccionar al menos un empleado")]
        [MinLength(1, ErrorMessage = "Debe seleccionar al menos un empleado")]
        public List<Guid> TargetEmployeeIds { get; set; } = new();

        // ═══════════════════════════════════════════════════════════════
        // DOCUMENTOS MAESTROS DEL LOTE
        // ═══════════════════════════════════════════════════════════════

        /// <summary>Documentos que aplican a todo el lote (no uno por empleado)</summary>
        [Required(ErrorMessage = "Debe adjuntar al menos un documento maestro")]
        public List<DocumentAttachmentDto> MasterDocuments { get; set; } = new();

        // ═══════════════════════════════════════════════════════════════
        // CONTROL DE FLUJO
        // ═══════════════════════════════════════════════════════════════

        /// <summary>Si es true, ejecuta inmediatamente. Si es false, guarda como Pending.</summary>
        public bool ExecuteImmediately { get; set; } = true;
    }
}
