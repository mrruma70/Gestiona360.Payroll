using System;
using System.Collections.Generic;
using System.Text;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    /// <summary>
    /// Datos de turnos de un empleado (turno actual + historial).
    /// </summary>
    public class EmployeeShiftsData
    {
        public string EmployeeName { get; set; } = string.Empty;
        public EmployeeCurrentShiftInfo? CurrentShift { get; set; }
        public List<EmployeeShiftHistoryInfo> ShiftHistory { get; set; } = new();
    }

    public class EmployeeCurrentShiftInfo
    {
        public Guid ShiftId { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public string ShiftType { get; set; } = string.Empty;
        public bool IsNightShift { get; set; }
        public DateTime StartDate { get; set; }
        public List<ShiftScheduleInfo> Schedules { get; set; } = new();
    }

    public class ShiftScheduleInfo
    {
        public int DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsRestDay { get; set; }
    }

    public class EmployeeShiftHistoryInfo
    {
        public Guid AssignmentId { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Justification { get; set; }
        public string ActionType { get; set; } = string.Empty;
    }
}
