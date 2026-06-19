using System;
using System.Collections.Generic;
using System.Text;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    /// <summary>
    /// Información básica de un centro de costo para listados simples.
    /// </summary>
    public class CostCenterBasicInfo
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CostType { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
