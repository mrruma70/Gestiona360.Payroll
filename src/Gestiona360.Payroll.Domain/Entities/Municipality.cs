using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Common;

namespace Gestiona360.Payroll.Domain.Entities
{
    public class Municipality : BaseEntityInt
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;


        public int Code { get; set; }

        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual ICollection<Neighborhood> Neighborhoods { get; set; } = new List<Neighborhood>();
    }
}
