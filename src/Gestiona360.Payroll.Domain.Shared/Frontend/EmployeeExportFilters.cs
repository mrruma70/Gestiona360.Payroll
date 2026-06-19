using System;
using System.Collections.Generic;
using System.Text;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    /// <summary>
    /// Filtros para la exportación de empleados.
    /// </summary>
    public class EmployeeExportFilters
    {
        public string? Search { get; set; }
        public Guid? BranchId { get; set; }
        public int? ContractTypeId { get; set; }
        public int? EmploymentStatus { get; set; }
        public Guid? JobPositionId { get; set; }
        public bool? IsTrustEmployee { get; set; }
        public bool? IsForeignWorker { get; set; }
        public bool? IsOnProbation { get; set; }
        public bool? IsRehire { get; set; }
        public Guid? PayrollGroupId { get; set; }
    }
}
