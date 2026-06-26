using System;
using System.Collections.Generic;
using System.Text;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    /// <summary>
    /// DTO para transportar datos de actualización de una acción de personal
    /// desde el Application Layer al Domain Service.
    /// </summary>
    public class PersonalActionUpdateData
    {
        /// <summary>ID de la acción a actualizar</summary>
        public Guid ActionId { get; set; }

        /// <summary>Fecha en que el cambio será efectivo</summary>
        public DateTime EffectiveDate { get; set; }

        /// <summary>Descripción de la causal legal</summary>
        public string CausalDescription { get; set; } = string.Empty;

        /// <summary>Justificación detallada del cambio</summary>
        public string Justification { get; set; } = string.Empty;

        /// <summary>Indica si se debe ejecutar inmediatamente</summary>
        public bool ExecuteImmediately { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // CAMPOS ESPECÍFICOS POR TIPO DE ACCIÓN
        // ═══════════════════════════════════════════════════════════════

        /// <summary>Nuevo salario base (para SalaryChange, PositionChange)</summary>
        public decimal? NewBaseSalary { get; set; }

        /// <summary>Nuevo ID de puesto/grado (para PositionChange)</summary>
        public Guid? NewJobGradeId { get; set; }

        /// <summary>Nuevo ID de tipo de contrato (para ContractChange)</summary>
        public int? NewContractTypeId { get; set; }

        /// <summary>Nuevo ID de turno (para ShiftChange)</summary>
        public Guid? NewShiftId { get; set; }

        /// <summary>Nuevo ID de centro de costo (para CostCenterChange)</summary>
        public Guid? NewCostCenterId { get; set; }

        /// <summary>Nuevo ID de banco (para BankAccountChange)</summary>
        public int? NewBankId { get; set; }

        /// <summary>Nuevo número de cuenta bancaria (para BankAccountChange)</summary>
        public string? NewBankAccountNumber { get; set; }

        /// <summary>Nuevo ID de proveedor de salud (para HealthProviderChange)</summary>
        public Guid? NewHealthProviderId { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // DOCUMENTOS ADJUNTOS
        // ═══════════════════════════════════════════════════════════════

        /// <summary>Lista de documentos adjuntos actualizados</summary>
        public List<DocumentAttachmentDto> Documents { get; set; } = new();
    }
}
