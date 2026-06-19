using System;
using System.Collections.Generic;
using System.Text;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    /// <summary>
    /// DTO para listado simplificado de empleados activos.
    /// </summary>
    public class ActiveEmployeeDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? JobGradeName { get; set; }
        public decimal BaseSalary { get; set; }
    }
}
