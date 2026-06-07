using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.Employees
{
    public class PersonalActionDto
    {
        // Campos básicos
        public Guid Id { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Justification { get; set; } = string.Empty;
        public string? CausalDescription { get; set; }

        // Auditoría
        public DateTime? ExecutedDate { get; set; }
        public Guid ApprovedBy { get; set; }
        public string ApprovedByName { get; set; } = string.Empty;

        // Valores fuertemente tipados (Old/New)
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

        // Compatibilidad con código antiguo (puedes eliminar si no lo usas)
        public string OldValue { get; set; } = string.Empty;
        public string NewValue { get; set; } = string.Empty;
    }
}
