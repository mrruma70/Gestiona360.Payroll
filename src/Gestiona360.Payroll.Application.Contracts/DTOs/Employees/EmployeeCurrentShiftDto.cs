using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.Employees
{
    public class EmployeeCurrentShiftDto
    {
        public Guid ShiftId { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public string ShiftType { get; set; } = string.Empty;
        public bool IsNightShift { get; set; }
        public DateTime StartDate { get; set; }
        public List<ShiftScheduleDto> Schedules { get; set; } = new();
    }
}
