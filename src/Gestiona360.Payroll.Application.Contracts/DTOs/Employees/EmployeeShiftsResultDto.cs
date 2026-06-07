using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.Employees
{
    public class EmployeeShiftsResultDto
    {

        
         public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public EmployeeCurrentShiftDto? CurrentShift { get; set; }
        public List<EmployeeShiftHistoryDto> ShiftHistory { get; set; } = new();
    }
}
