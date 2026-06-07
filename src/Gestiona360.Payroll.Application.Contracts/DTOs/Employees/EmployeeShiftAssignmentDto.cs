using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.Employees
{
    public class EmployeeShiftAssignmentDto
    {
        public Guid Id { get; set; }
        public Guid ShiftId { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public string ShiftType { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public decimal TotalHours { get; set; }
        public bool IsNightShift { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Justification { get; set; } = string.Empty;
        public bool IsActive => !EndDate.HasValue || EndDate > DateTime.UtcNow;
    }

}
