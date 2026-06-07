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
    public class IndemnityProvision : BaseEntityGuid
    {
        public Guid EmployeeId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyProvision { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAccrued { get; set; }

        [Required, MaxLength(10)]
        public string Sector { get; set; } = "Privado"; // Privado / Público

        public DateTime LastProvisionDate { get; set; }

        public virtual Employee Employee { get; set; }
    }
}
