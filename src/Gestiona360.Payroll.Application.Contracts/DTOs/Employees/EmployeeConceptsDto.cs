using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.Employees
{
    public class EmployeeConceptsResultDto
    {
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public List<EmployeeConceptLineDto> Concepts { get; set; } = new();
        public List<EmployeeGarnishmentLineDto> Garnishments { get; set; } = new();
    }
}
