using Gestiona360.Payroll.Domain.Common;
using Gestiona360.Payroll.Domain.Shared.Frontend.Enums;

namespace Gestiona360.Payroll.Domain.Entities
{
    public class PersonalAction : BaseEntityGuid
    {
        // 🔗 Relaciones
        public Guid EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        // 🏢 Grupo de Nómina donde se procesó la acción
        public Guid PayrollGroupId { get; set; }
        public virtual PayrollGroup PayrollGroup { get; set; }

        // 📅 Período de Nómina específico (se llena AUTOMÁTICAMENTE al crear)
        public Guid TargetPayrollPeriodId { get; set; }
        public virtual PayrollPeriod TargetPayrollPeriod { get; set; }

        // 📋 Datos de la Acción
        public ActionType ActionType { get; set; }
        public DateTime EffectiveDate { get; set; }
        public ActionStatus Status { get; set; }

        // ⚖️ Causal legal explícita
        public string? CausalDescription { get; set; }

        // 👤 Auditoría de Ejecución
        public Guid ApprovedBy { get; set; }
        public DateTime? ExecutedDate { get; set; }

        // 📝 Justificación y Soporte
        public string Justification { get; set; } = string.Empty;
        public string DocumentAttachments { get; set; } = "[]";

        // 🔗 Vinculación para Eventos Masivos
        public string? BatchReference { get; set; }

        // 🔄 CONTROL DE PROCESAMIENTO DE NÓMINA (NUEVO)
        /// <summary>
        /// Indica si esta acción ya fue procesada/aplicada en el cálculo de nómina.
        /// Se establece en TRUE automáticamente cuando el motor de nómina procesa el período.
        /// </summary>
        public bool IsAppliedInPayroll { get; set; } = false;

        /// <summary>
        /// Referencia al PayrollRecord específico donde se aplicó esta acción.
        /// Permite trazabilidad completa: saber exactamente en qué colilla se reflejó el cambio.
        /// </summary>
        public Guid? AppliedInPayrollRecordId { get; set; }
        public virtual PayrollRecord? AppliedInPayrollRecord { get; set; }

        /// <summary>
        /// Fecha/hora en que la nómina procesó esta acción.
        /// </summary>
        public DateTime? AppliedInPayrollDate { get; set; }

        // 💰 VALORES FUERTEMENTE TIPADOS
        public decimal? OldBaseSalary { get; set; }
        public decimal? NewBaseSalary { get; set; }

        public Guid? OldJobGradeId { get; set; }
        public Guid? NewJobGradeId { get; set; }

        public int? OldContractTypeId { get; set; }
        public int? NewContractTypeId { get; set; }

        public Guid? OldShiftId { get; set; }
        public Guid? NewShiftId { get; set; }

        public Guid? OldCostCenterId { get; set; }
        public Guid? NewCostCenterId { get; set; }

        public int? OldEmploymentStatus { get; set; }
        public int? NewEmploymentStatus { get; set; }
    }
}
