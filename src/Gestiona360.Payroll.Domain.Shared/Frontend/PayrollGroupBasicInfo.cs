using System;
using System.Collections.Generic;
using System.Text;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    /// <summary>
    /// Información básica de un grupo de nómina para listados simples.
    /// </summary>
    public class PayrollGroupBasicInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
