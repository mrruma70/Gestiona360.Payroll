using System;
using System.Collections.Generic;
using System.Text;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    /// <summary>
    /// DTO interno para transportar datos de la acción masiva al servicio de dominio.
    /// </summary>
    public class MassPersonalActionData
    {
        public List<Guid> TargetEmployeeIds { get; set; } = new();
        public DateTime EffectiveDate { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public string? RuleType { get; set; }
        public decimal? RuleValue { get; set; }
        public string? CausalDescription { get; set; }
        public string? Justification { get; set; }
        public string BatchReference { get; set; } = string.Empty;
        public List<string>? MasterDocuments { get; set; }
    }
}
