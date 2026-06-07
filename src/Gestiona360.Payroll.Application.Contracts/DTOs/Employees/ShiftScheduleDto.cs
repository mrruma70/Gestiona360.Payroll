using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.Employees
{
    public class ShiftScheduleDto
    {
        public int DayOfWeek { get; set; } // 0=Domingo, 1=Lunes, ..., 6=Sábado
        public string DayName { get; set; } = string.Empty;
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public bool IsRestDay { get; set; }
    }
}
