using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs
{
    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Identification { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public decimal BaseSalary { get; set; }
        public string JobGradeName { get; set; } = string.Empty;
        public string ShiftName { get; set; } = string.Empty;
        public string CostCenterCode { get; set; } = string.Empty;
      
    }
}
