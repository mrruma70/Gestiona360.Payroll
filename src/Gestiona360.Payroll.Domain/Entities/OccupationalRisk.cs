using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Common;

namespace Gestiona360.Payroll.Domain.Entities
{
    public class OccupationalRisk : BaseEntityInt
    {
        [Required, MaxLength(10)]
        public string Code { get; set; }          // R01, R02, R03

        [Required, MaxLength(50)]
        public string Name { get; set; }          // Bajo / Medio / Alto

        public string Description { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal INSSRiskRate { get; set; } // Prima adicional (0, 0.5, 1.0 %)

        [MaxLength(50)]
        public string LegalReference { get; set; }
    }
}
