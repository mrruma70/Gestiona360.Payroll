using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.PersonalActions
{
    public class CreatePersonalActionRequest
    {
        public Guid EmployeeId { get; set; }
        public string ActionType { get; set; } = string.Empty; // Enum como string para UI
        public DateTime EffectiveDate { get; set; }
        public string Justification { get; set; } = string.Empty;
        public List<Guid> DocumentIds { get; set; } = new(); // IDs de documentos subidos

        // Campos Dinámicos según ActionType
        public decimal? NewBaseSalary { get; set; }
        public Guid? NewJobGradeId { get; set; }
        public int? NewContractTypeId { get; set; }
        public Guid? NewShiftId { get; set; }
        public Guid? NewCostCenterId { get; set; }

        // Para Suspensiones/Terminaciones
        public DateTime? EndDate { get; set; }
        public string? MitrabAuthorization { get; set; }
        public bool? IsJustified { get; set; }

        // ==========================================================
        // HEALTH PROVIDER CHANGE
        // ==========================================================
        public Guid? NewHealthProviderId { get; set; }
        public string? NewHealthProviderName { get; set; }

        // ==========================================================
        // BANK ACCOUNT CHANGE
        // ==========================================================
        public int? NewBankId { get; set; }
        public string? NewBankName { get; set; }
        public string? NewBankAccountNumber { get; set; }
    }
}
