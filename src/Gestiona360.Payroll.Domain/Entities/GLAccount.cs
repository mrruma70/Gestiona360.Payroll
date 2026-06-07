using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Common;

namespace Gestiona360.Payroll.Domain.Entities
{
    public class GLAccount : BaseEntityGuid
    {
        [Required, MaxLength(20)]
        public string Code { get; set; }            // 11001-01

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, MaxLength(20)]
        public string AccountType { get; set; }     // Activo / Pasivo / Costo / Gasto / Ingreso

        public bool IsActive { get; set; }
    }
}
