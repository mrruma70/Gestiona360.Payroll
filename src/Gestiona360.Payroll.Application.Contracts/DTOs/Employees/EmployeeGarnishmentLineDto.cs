using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.Employees
{
    public class EmployeeGarnishmentLineDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty; // Alimony, Civil
        public string CourtOrderNumber { get; set; } = string.Empty;
        public decimal PercentageLimit { get; set; }
        public decimal TotalAmountToWithhold { get; set; }
        public decimal WithheldToDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
