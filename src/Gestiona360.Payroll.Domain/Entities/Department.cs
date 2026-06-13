using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Common;

namespace Gestiona360.Payroll.Domain.Entities
{
    public class Department : BaseEntityInt
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(10)]
        public string? Code { get; set; }


        public virtual ICollection<Municipality> Municipalities { get; set; } = new List<Municipality>();
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
