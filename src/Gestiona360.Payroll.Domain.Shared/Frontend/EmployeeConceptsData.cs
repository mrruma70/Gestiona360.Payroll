using System;
using System.Collections.Generic;
using System.Text;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    /// <summary>
    /// Datos de conceptos y retenciones de un empleado.
    /// </summary>
    public class EmployeeConceptsData
    {
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public List<EmployeeConceptInfo> Concepts { get; set; } = new();
        public List<GarnishmentInfo> Garnishments { get; set; } = new();
    }

    public class EmployeeConceptInfo
    {
        public Guid Id { get; set; }
        public string ConceptName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public decimal Amount { get; set; }
        public string Periodicity { get; set; } = string.Empty;
        public string? InstallmentsInfo { get; set; }
        public decimal? RemainingBalance { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class GarnishmentInfo
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string CourtOrderNumber { get; set; } = string.Empty;
        public decimal PercentageLimit { get; set; }
        public decimal TotalAmountToWithhold { get; set; }
        public decimal WithheldToDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
