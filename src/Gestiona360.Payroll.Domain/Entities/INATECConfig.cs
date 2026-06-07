using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Common;

namespace Gestiona360.Payroll.Domain.Entities
{
    public class INATECConfig : BaseEntityInt
    {


        [Required]
        public DateOnly EffectiveFrom { get; set; }

        public DateOnly? EffectiveTo { get; set; }

        [Required, MaxLength(150)]
        public string LegalReference { get; set; } = "Ley 1063";

        [Required, Range(0, 100)]
        public decimal Rate { get; set; }

        [MaxLength(200)]
        public string Exceptions { get; set; } = "MINED, MINSA";



        public int Year => EffectiveFrom.Year;
    }
}
