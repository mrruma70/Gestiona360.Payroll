using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.PersonalActions
{
    /// <summary>
    /// DTO para crear una Acción de Personal individual.
    /// Solo se deben llenar los campos Old/New relevantes según el ActionType.
    /// </summary>
    public class CreatePersonalActionDto
    {
        // ═══════════════════════════════════════════════════════════════
        // DATOS OBLIGATORIOS
        // ═══════════════════════════════════════════════════════════════

        [Required(ErrorMessage = "El empleado es obligatorio")]
        public Guid EmployeeId { get; set; }

        [Required(ErrorMessage = "El tipo de acción es obligatorio")]
        public string ActionType { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de efecto es obligatoria")]
        public DateTime EffectiveDate { get; set; }

        [Required(ErrorMessage = "La justificación es obligatoria")]
        [MinLength(20, ErrorMessage = "La justificación debe tener al menos 20 caracteres")]
        [MaxLength(2000, ErrorMessage = "La justificación no puede exceder 2000 caracteres")]
        public string Justification { get; set; } = string.Empty;

        // ═══════════════════════════════════════════════════════════════
        // CAUSAL LEGAL (Obligatoria para Suspension/Termination)
        // ═══════════════════════════════════════════════════════════════

        /// <summary>Descripción de la causal legal (ej: "Ascenso por mérito")</summary>
        [MaxLength(200)]
        public string? CausalDescription { get; set; }

        /// <summary>Tipo de suspensión (enum como string, requerido si ActionType = Suspension)</summary>
        public string? SuspensionType { get; set; }

        /// <summary>Tipo de terminación (enum como string, requerido si ActionType = Termination)</summary>
        public string? TerminationType { get; set; }

        /// <summary>Indica si el despido es justificado (Art. 48)</summary>
        public bool? IsJustified { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // FECHAS DE SUSPENSIÓN (si aplica)
        // ═══════════════════════════════════════════════════════════════

        public DateTime? SuspensionStartDate { get; set; }
        public DateTime? SuspensionEndDate { get; set; }

        /// <summary>N° de autorización MITRAB (obligatorio para MassForceMajeure)</summary>
        [MaxLength(100)]
        public string? MitrabAuthorizationNumber { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // VALORES FUERTEMENTE TIPADOS (Old/New)
        // ═══════════════════════════════════════════════════════════════

        // --- Salario ---
        [Range(0, 10000000, ErrorMessage = "El salario debe estar entre 0 y 10,000,000")]
        public decimal? OldBaseSalary { get; set; }

        [Range(0, 10000000, ErrorMessage = "El salario debe estar entre 0 y 10,000,000")]
        public decimal? NewBaseSalary { get; set; }

        // --- Puesto ---
        public Guid? OldJobGradeId { get; set; }
        public Guid? NewJobGradeId { get; set; }

        // --- Contrato ---
        public int? OldContractTypeId { get; set; }
        public int? NewContractTypeId { get; set; }

        // --- Turno ---
        public Guid? OldShiftId { get; set; }
        public Guid? NewShiftId { get; set; }

        // --- Centro de Costo ---
        public Guid? OldCostCenterId { get; set; }
        public Guid? NewCostCenterId { get; set; }

        public Guid? NewHealthProviderId { get; set; }

        

        // ═══════════════════════════════════════════════════════════════
        // DOCUMENTOS ADJUNTOS
        // ═══════════════════════════════════════════════════════════════

        /// <summary>Lista de URLs de documentos adjuntos (subidos previamente)</summary>
        public List<DocumentAttachmentDto> Documents { get; set; } = new();

        // ═══════════════════════════════════════════════════════════════
        // CONTROL DE FLUJO
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Si es true, la acción se ejecuta inmediatamente al crear.
        /// Si es false, se guarda como Pending para revisión posterior.
        /// </summary>
        public bool ExecuteImmediately { get; set; } = false;

        // Para BankAccountChange
        public int? NewBankId { get; set; }
        public string? NewBankAccountNumber { get; set; }
    }
}
