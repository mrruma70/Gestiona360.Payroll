using System;
using System.Collections.Generic;
using System.Text;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    public class INSSConfigDto
    {
        public int Year { get; set; }
        public decimal RateWorker { get; set; }
        public decimal RateEmployerSmall { get; set; }
        public decimal RateEmployerLarge { get; set; }
        public decimal MaxSalaryCap { get; set; }
        public string LegalReference { get; set; } = string.Empty;
        public DateOnly EffectiveFrom { get; set; }      // <-- Corregido a DateOnly
        public DateOnly? EffectiveTo { get; set; }       // <-- Corregido a DateOnly?
    }
}
