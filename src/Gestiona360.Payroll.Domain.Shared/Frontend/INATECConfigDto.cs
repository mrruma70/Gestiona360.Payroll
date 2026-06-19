using System;
using System.Collections.Generic;
using System.Text;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    public class INATECConfigDto
    {
        public int Year { get; set; }
        public decimal Rate { get; set; }
        public string Exceptions { get; set; } = string.Empty;
        public string LegalReference { get; set; } = string.Empty;
        public DateOnly EffectiveFrom { get; set; }      // <-- Corregido a DateOnly
        public DateOnly? EffectiveTo { get; set; }       // <-- Corregido a DateOnly?
    }
}
