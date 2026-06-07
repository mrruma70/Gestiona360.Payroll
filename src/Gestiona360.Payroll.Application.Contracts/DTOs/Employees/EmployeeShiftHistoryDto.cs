using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.Employees
{
    public class EmployeeShiftHistoryDto
    {
        public Guid AssignmentId { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Justification { get; set; } = string.Empty;
        public string ActionType { get; set; } = string.Empty; // Ej: "Contratación", "Cambio de Turno"
    }
}
