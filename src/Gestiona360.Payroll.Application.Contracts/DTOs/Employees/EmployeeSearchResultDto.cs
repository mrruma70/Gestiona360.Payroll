using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.Employees
{
    public class EmployeeSearchResultDto
    {
        public List<EmployeeListDto> Employees { get; set; } = new();
        public EmployeeStatsDto Stats { get; set; } = new();
    }
}
