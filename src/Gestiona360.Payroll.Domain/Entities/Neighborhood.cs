using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Common;

namespace Gestiona360.Payroll.Domain.Entities
{
    public class Neighborhood : BaseEntityInt
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public int MunicipalityId { get; set; }
        public virtual Municipality Municipality { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
