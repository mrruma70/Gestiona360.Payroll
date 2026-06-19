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
        // DATOS BÁSICOS (hereda de ListDto)
        // ═══════════════════════════════════════════════════════════════

        public Guid Id { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string? BatchReference { get; set; }
        public int? AffectedCount { get; set; }

        public string? EmployeeCode { get; set; }
        public string? EmployeeName { get; set; }
        public string? EmployeeIdentification { get; set; }
        public string? EmployeePosition { get; set; }

        public string ActionType { get; set; } = string.Empty;
        public string ActionTypeDescription { get; set; } = string.Empty;
        public string? CausalDescription { get; set; }

        public string Status { get; set; } = string.Empty;

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

        /// <summary>Lista de URLs/nombres de archivos adjuntos</summary>
        public List<DocumentAttachmentDto> DocumentAttachments { get; set; } = new();

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

        // ═══════════════════════════════════════════════════════════════
        // VALORES FUERTEMENTE TIPADOS (comparativa completa)
        // ═══════════════════════════════════════════════════════════════

        public decimal? OldBaseSalary { get; set; }
        public decimal? NewBaseSalary { get; set; }

        public Guid? OldJobGradeId { get; set; }
        public string? OldJobGradeName { get; set; }
        public Guid? NewJobGradeId { get; set; }
        public string? NewJobGradeName { get; set; }

        public int? OldContractTypeId { get; set; }
        public string? OldContractTypeName { get; set; }
        public int? NewContractTypeId { get; set; }
        public string? NewContractTypeName { get; set; }

        public Guid? OldShiftId { get; set; }
        public string? OldShiftName { get; set; }
        public Guid? NewShiftId { get; set; }
        public string? NewShiftName { get; set; }

        public Guid? OldCostCenterId { get; set; }
        public string? OldCostCenterName { get; set; }
        public Guid? NewCostCenterId { get; set; }
        public string? NewCostCenterName { get; set; }

        public string? OldEmploymentStatusName { get; set; }
        public string? NewEmploymentStatusName { get; set; }


        public string? OldHealthProviderName { get; set; }
        public string? NewHealthProviderName { get; set; }
        public Guid? OldHealthProviderId { get; set; }
        public Guid? NewHealthProviderId { get; set; }

        public string? OldBankName { get; set; }
        public string? NewBankName { get; set; }
        public int? OldBankId { get; set; }
        public int? NewBankId { get; set; }
        public string? OldBankAccountNumber { get; set; }
        public string? NewBankAccountNumber { get; set; }

        public string? SuspensionType { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // DATOS ESPECÍFICOS DE SUSPENSIÓN
        // ═══════════════════════════════════════════════════════════════

        public DateTime? SuspensionStartDate { get; set; }
        public DateTime? SuspensionEndDate { get; set; }
        public string? MitrabAuthorizationNumber { get; set; }
        public string? SuspensionTypeName { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // DATOS ESPECÍFICOS DE TERMINACIÓN
        // ═══════════════════════════════════════════════════════════════

        public string? TerminationTypeName { get; set; }
        public bool? IsJustified { get; set; }
        public decimal? IndemnityAmount { get; set; }
        public decimal? TotalNetPayment { get; set; }

        // Campo temporal para parsear documentos
        public string? DocumentAttachmentsJson { get; set; }

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
