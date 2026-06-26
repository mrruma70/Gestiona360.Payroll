using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    public class EmployeeSearchDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        // Datos para el resumen
        public decimal BaseSalary { get; set; }
        public string JobGradeName { get; set; } = string.Empty;
        public string CostCenterCode { get; set; } = string.Empty;
        public string ShiftName { get; set; } = string.Empty;
        public DateTime? HireDate { get; set; } // Hice nullable por seguridad

        // Propiedad calculada para el formato de búsqueda
        public string DisplayName => $"{FullName} ({Code})";

        public string Identification { get; set; } = string.Empty;
    }
}
