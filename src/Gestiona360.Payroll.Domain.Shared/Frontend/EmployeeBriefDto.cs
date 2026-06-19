using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    /// <summary>
    /// DTO resumido de empleado para listas (ej: empleados afectados en lote masivo).
    /// </summary>
    public class EmployeeBriefDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Identification { get; set; }
        public string? Position { get; set; }
    }
}
