using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Shared.Frontend.Enums;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    /// <summary>
    /// Entidad de transporte para acciones personales con detalles relacionados.
    /// No se persiste en BD, solo se usa para transporte de datos desde Infrastructure.
    /// </summary>
    public class PersonalActionWithDetails
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public DateTime EffectiveDate { get; set; }

        // ✅ CORREGIDO: ActionType (no PersonalActionType)
        public ActionType ActionType { get; set; }

        // ✅ CORREGIDO: ActionStatus (no PersonalActionStatus)
        public ActionStatus Status { get; set; }

        public string? Justification { get; set; }
        public string? CausalDescription { get; set; }
        public DateTime? ExecutedDate { get; set; }
        public Guid? ApprovedBy { get; set; }
        public string? ApprovedByName { get; set; }

        // Salario
        public decimal? OldBaseSalary { get; set; }
        public decimal? NewBaseSalary { get; set; }

        // Puesto
        public Guid? OldJobGradeId { get; set; }
        public string? OldJobGradeName { get; set; }
        public Guid? NewJobGradeId { get; set; }
        public string? NewJobGradeName { get; set; }

        // Contrato
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
    }
}
