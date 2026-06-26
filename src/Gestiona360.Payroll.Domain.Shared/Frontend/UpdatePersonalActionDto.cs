using System;
using System.Collections.Generic;
using System.Text;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    public class UpdatePersonalActionDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public string CausalDescription { get; set; } = string.Empty;
        public string Justification { get; set; } = string.Empty;
        public bool ExecuteImmediately { get; set; }

        // Campos específicos por tipo de acción
        public decimal? OldBaseSalary { get; set; }
        public decimal? NewBaseSalary { get; set; }
        public Guid? NewJobGradeId { get; set; }
        public int? NewContractTypeId { get; set; }
        public Guid? NewShiftId { get; set; }
        public Guid? NewCostCenterId { get; set; }
        public Guid? NewHealthProviderId { get; set; }
        public int? NewBankId { get; set; }
        public string? NewBankAccountNumber { get; set; }

        public List<DocumentAttachmentDto> Documents { get; set; } = new();
    }
}
