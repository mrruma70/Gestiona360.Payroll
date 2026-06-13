using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.PersonalActions
{
    /// <summary>
    /// DTO para el listado principal de Acciones de Personal (MudDataGrid).
    /// Contiene los campos necesarios para mostrar la tabla con filtros y acciones contextuales.
    /// </summary>
    public class PersonalActionListDto
    {
        // ═══════════════════════════════════════════════════════════════
        // IDENTIFICADORES Y TRAZABILIDAD
        // ═══════════════════════════════════════════════════════════════

        /// <summary>ID único de la acción de personal</summary>
        public Guid Id { get; set; }

        /// <summary>Fecha en que el cambio debe aplicarse</summary>
        public DateTime EffectiveDate { get; set; }

        /// <summary>Referencia de lote (no nulo si es acción masiva)</summary>
        public string? BatchReference { get; set; }

        /// <summary>Cantidad de empleados afectados (solo si es masiva)</summary>
        public int? AffectedCount { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // DATOS DEL EMPLEADO (o del lote si es masivo)
        // ═══════════════════════════════════════════════════════════════

        /// <summary>Código del empleado (ej: EMP-001). Null si es masiva.</summary>
        public string? EmployeeCode { get; set; }

        /// <summary>Nombre completo del empleado. Null si es masiva.</summary>
        public string? EmployeeName { get; set; }

        /// <summary>Cédula del empleado. Null si es masiva.</summary>
        public string? EmployeeIdentification { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // TIPO Y CAUSAL DE LA ACCIÓN
        // ═══════════════════════════════════════════════════════════════

        /// <summary>Tipo de acción como string (ej: "SalaryChange")</summary>
        public string ActionType { get; set; } = string.Empty;

        /// <summary>Descripción legible del tipo (ej: "💰 Cambio de Salario")</summary>
        public string ActionTypeDescription { get; set; } = string.Empty;

        /// <summary>Causal legal específica (ej: "Ascenso por mérito")</summary>
        public string? CausalDescription { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // ESTADO Y AUDITORÍA
        // ═══════════════════════════════════════════════════════════════

        /// <summary>Estado actual como string (ej: "Pending", "Executed")</summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>Nombre del usuario que aprobó/ejecutó</summary>
        public string? ApprovedByName { get; set; }

        /// <summary>Fecha de ejecución (null si aún no se ejecuta)</summary>
        public DateTime? ExecutedDate { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // VALORES FUERTEMENTE TIPADOS (Old/New) - Para mostrar comparativa
        // ═══════════════════════════════════════════════════════════════

        // --- Salario ---
        public decimal? OldBaseSalary { get; set; }
        public decimal? NewBaseSalary { get; set; }

        // --- Puesto (solo IDs para lógica, nombres para UI) ---
        public Guid? OldJobGradeId { get; set; }
        public string? OldJobGradeName { get; set; }
        public Guid? NewJobGradeId { get; set; }
        public string? NewJobGradeName { get; set; }

        // --- Tipo de Contrato ---
        public int? OldContractTypeId { get; set; }
        public string? OldContractTypeName { get; set; }
        public int? NewContractTypeId { get; set; }
        public string? NewContractTypeName { get; set; }

        // --- Turno ---
        public Guid? OldShiftId { get; set; }
        public string? OldShiftName { get; set; }
        public Guid? NewShiftId { get; set; }
        public string? NewShiftName { get; set; }

        // --- Centro de Costo ---
        public Guid? OldCostCenterId { get; set; }
        public string? OldCostCenterName { get; set; }
        public Guid? NewCostCenterId { get; set; }
        public string? NewCostCenterName { get; set; }

        // --- Estado Laboral (para Suspension/Termination) ---
        public string? OldEmploymentStatusName { get; set; }
        public string? NewEmploymentStatusName { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // CAMPOS DE SUSPENSIÓN (solo si ActionType = Suspension/MassSuspension)
        // ═══════════════════════════════════════════════════════════════

        public DateTime? SuspensionStartDate { get; set; }
        public DateTime? SuspensionEndDate { get; set; }
        public string? MitrabAuthorizationNumber { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // CONTROL DE PROCESAMIENTO DE NÓMINA
        // ═══════════════════════════════════════════════════════════════

        /// <summary>Indica si la nómina ya procesó esta acción</summary>
        public bool IsAppliedInPayroll { get; set; }

        /// <summary>Fecha en que la nómina procesó la acción</summary>
        public DateTime? AppliedInPayrollDate { get; set; }
    }
}
