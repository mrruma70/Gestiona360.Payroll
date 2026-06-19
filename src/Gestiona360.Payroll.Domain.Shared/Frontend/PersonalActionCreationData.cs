using System;
using System.Collections.Generic;
using System.Text;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    /// <summary>
    /// DTO interno para transportar datos de creación de acción al servicio de dominio.
    /// </summary>
    public class PersonalActionCreationData
    {
        public Guid EmployeeId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public string? CausalDescription { get; set; }
        public string Justification { get; set; } = string.Empty;
        public bool ExecuteImmediately { get; set; }

        // NewValues
        public decimal? NewBaseSalary { get; set; }
        public Guid? NewJobGradeId { get; set; }
        public int? NewContractTypeId { get; set; }
        public Guid? NewCostCenterId { get; set; }
        public Guid? NewShiftId { get; set; }
        public int? NewBankId { get; set; }
        public string? NewBankAccountNumber { get; set; }
        public Guid? NewHealthProviderId { get; set; }

        // Documentos
        public List<DocumentAttachmentDto>? Documents { get; set; }
    }
}
