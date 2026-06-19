using System;
using System.Collections.Generic;
using System.Text;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    /// <summary>
    /// Información básica de un empleado para listados simples.
    /// </summary>
    public class EmployeeBasicInfo
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Identification { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
