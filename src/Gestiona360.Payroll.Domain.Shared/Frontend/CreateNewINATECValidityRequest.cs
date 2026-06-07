using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    public class CreateNewINATECValidityRequest
    {
        [Required]
        public DateOnly EffectiveFrom { get; set; }

        [Required]
        [MaxLength(150)]
        public string LegalReference { get; set; } = string.Empty;

        [Required]
        [Range(0, 100)]
        public decimal Rate { get; set; }

        [MaxLength(200)]
        public string Exceptions { get; set; } = "MINED, MINSA";
    }
}
