using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Common;

namespace Gestiona360.Payroll.Domain.Entities
{
    public class ZFBenefit : BaseEntityGuid
    {
        public Guid CompanyId { get; set; }
        public DateTime StartDate { get; set; }
        public int YearsExempt { get; set; }

        [Required, MaxLength(20)]
        public string Type { get; set; }   // Operadora / Usuaria

        public virtual Company Company { get; set; }
    }
}
