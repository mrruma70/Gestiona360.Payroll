using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Shared.Frontend;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    /// <summary>
    /// DTO completo para el Drawer lateral de detalle de Acción de Personal.
    /// Incluye toda la información de auditoría, documentos y lista de empleados afectados (si es masiva).
    /// </summary>
    public class PersonalActionDetailDto
    {
        // ═══════════════════════════════════════════════════════════════
        // IDENTIFICADORES Y DATOS BÁSICOS
        // ═══════════════════════════════════════════════════════════════

        public Guid Id { get; set; }

        /// <summary>ID del empleado (necesario para edición)</summary>
        public Guid EmployeeId { get; set; }

        public DateTime EffectiveDate { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public string ActionTypeDescription { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? CausalDescription { get; set; }

        // Datos masivos (solo si BatchReference != null)
        public string? BatchReference { get; set; }
        public int? AffectedCount { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // DATOS DEL EMPLEADO (snapshot al momento de la acción)
        // ═══════════════════════════════════════════════════════════════

        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeIdentification { get; set; }
        public string? EmployeePosition { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // CONTEXTO DE NÓMINA
        // ═══════════════════════════════════════════════════════════════

        public Guid PayrollGroupId { get; set; }
        public string PayrollGroupName { get; set; } = string.Empty;
        public Guid TargetPayrollPeriodId { get; set; }
        public string PayrollPeriodName { get; set; } = string.Empty;
        public string PayrollPeriodStatus { get; set; } = string.Empty;

        // ═══════════════════════════════════════════════════════════════
        // JUSTIFICACIÓN Y DOCUMENTOS
        // ═══════════════════════════════════════════════════════════════

        /// <summary>Motivo detallado de la acción</summary>
        public string Justification { get; set; } = string.Empty;

        /// <summary>Lista de documentos adjuntos</summary>
        public List<DocumentAttachmentDto> Documents { get; set; } = new();

        /// <summary>Alias para compatibilidad con el nombre DocumentAttachments</summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public List<DocumentAttachmentDto> DocumentAttachments
        {
            get => Documents;
            set => Documents = value ?? new();
        }

        /// <summary>Campo temporal para parsear documentos desde JSON string</summary>
        [System.Text.Json.Serialization.JsonIgnore]
        public string? DocumentAttachmentsJson { get; set; }

        /// <summary>Motivo del rechazo (solo si Status = Rejected)</summary>
        public string? RejectionReason { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // AUDITORÍA COMPLETA
        // ═══════════════════════════════════════════════════════════════

        public DateTime CreatedDate { get; set; }
        public string CreatedByName { get; set; } = string.Empty;

        public DateTime? ApprovedDate { get; set; }
        public string? ApprovedByName { get; set; }

        public DateTime? ExecutedDate { get; set; }
        public string? ExecutedByName { get; set; }

        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedByName { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // VALORES FUERTEMENTE TIPADOS (comparativa Old vs New)
        // ═══════════════════════════════════════════════════════════════

        // Salario
        public decimal? OldBaseSalary { get; set; }
        public decimal? NewBaseSalary { get; set; }

        // Puesto / Grado
        public Guid? OldJobGradeId { get; set; }
        public string? OldJobGradeName { get; set; }
        public Guid? NewJobGradeId { get; set; }
        public string? NewJobGradeName { get; set; }

        // Tipo de Contrato
        public int? OldContractTypeId { get; set; }
        public string? OldContractTypeName { get; set; }
        public int? NewContractTypeId { get; set; }
        public string? NewContractTypeName { get; set; }

        // Turno
        public Guid? OldShiftId { get; set; }
        public string? OldShiftName { get; set; }
        public Guid? NewShiftId { get; set; }
        public string? NewShiftName { get; set; }

        // Centro de Costo
        public Guid? OldCostCenterId { get; set; }
        public string? OldCostCenterName { get; set; }
        public Guid? NewCostCenterId { get; set; }
        public string? NewCostCenterName { get; set; }

        // Estado de Empleo
        public string? OldEmploymentStatusName { get; set; }
        public string? NewEmploymentStatusName { get; set; }

        // Proveedor de Salud
        public Guid? OldHealthProviderId { get; set; }
        public string? OldHealthProviderName { get; set; }
        public Guid? NewHealthProviderId { get; set; }
        public string? NewHealthProviderName { get; set; }

        // Banco y Cuenta Bancaria
        public int? OldBankId { get; set; }
        public string? OldBankName { get; set; }
        public string? OldBankAccountNumber { get; set; }
        public int? NewBankId { get; set; }
        public string? NewBankName { get; set; }
        public string? NewBankAccountNumber { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // DATOS ESPECÍFICOS DE SUSPENSIÓN
        // ═══════════════════════════════════════════════════════════════

        /// <summary>Código del tipo de suspensión (MedicalLeave, OccupationalRisk, etc.)</summary>
        public string? SuspensionType { get; set; }

        /// <summary>Nombre legible del tipo de suspensión</summary>
        public string? SuspensionTypeName { get; set; }

        public DateTime? SuspensionStartDate { get; set; }
        public DateTime? SuspensionEndDate { get; set; }
        public string? MitrabAuthorizationNumber { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // DATOS ESPECÍFICOS DE TERMINACIÓN
        // ═══════════════════════════════════════════════════════════════

        /// <summary>Código del tipo de terminación (Resignation, UnfairDismissal, etc.)</summary>
        public string? TerminationType { get; set; }

        /// <summary>Nombre legible del tipo de terminación</summary>
        public string? TerminationTypeName { get; set; }

        public bool? IsJustified { get; set; }
        public decimal? IndemnityAmount { get; set; }
        public decimal? TotalNetPayment { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // LISTA DE EMPLEADOS AFECTADOS (solo si es acción masiva)
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Lista de empleados afectados en el lote.
        /// Solo se llena si BatchReference != null.
        /// </summary>
        public List<EmployeeBriefDto> AffectedEmployees { get; set; } = new();

        // ═══════════════════════════════════════════════════════════════
        // CONTROL DE PROCESAMIENTO DE NÓMINA
        // ═══════════════════════════════════════════════════════════════

        public bool IsAppliedInPayroll { get; set; }
        public DateTime? AppliedInPayrollDate { get; set; }
        public Guid? AppliedInPayrollRecordId { get; set; }
    }

}
