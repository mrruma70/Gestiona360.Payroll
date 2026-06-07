using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.Employees
{
    public class EmployeeConceptLineDto
    {
        public Guid Id { get; set; }
        public string ConceptName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Perception / Deduction
        public string Category { get; set; } = string.Empty; // Legal, Contractual, Judicial, etc.
        public bool IsActive { get; set; }
        public decimal Amount { get; set; }
        public string Periodicity { get; set; } = string.Empty; // Monthly, Biweekly, etc.
        public string? InstallmentsInfo { get; set; } // Ej: "3 de 5"
        public decimal? RemainingBalance { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
