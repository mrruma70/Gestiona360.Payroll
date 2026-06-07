using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs
{
    public class INATECConfigDto
    {
        public int Year { get; set; }
        public decimal Rate { get; set; }
        public string Exceptions { get; set; } = string.Empty;
        public string LegalReference { get; set; } = string.Empty;
        public DateOnly EffectiveFrom { get; set; }
        public DateOnly? EffectiveTo { get; set; }
    }
}
