using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.Employees
{
    public class AttendanceRecordDto
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan? CheckIn { get; set; }
        public TimeSpan? CheckOut { get; set; }
        public decimal TotalHours { get; set; }
        public decimal OvertimeHours { get; set; }
        public decimal NightHours { get; set; }
        public decimal HolidayHours { get; set; }
        public bool IsVacationDay { get; set; }
        public bool IsSickLeave { get; set; }
        public bool IsJustifiedAbsence { get; set; }
    }
}
