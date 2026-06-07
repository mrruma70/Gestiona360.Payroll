using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Common;

namespace Gestiona360.Payroll.Domain.Entities
{
    public class HealthProvider : BaseEntityGuid
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, MaxLength(20)]
        public string Type { get; set; }          // Aseguradora / Clínica / Hospital

        [Phone, MaxLength(20)]
        public string ContactPhone { get; set; }

        [MaxLength(200)]
        public string Address { get; set; }

        public Guid CompanyId { get; set; }
        public bool IsActive { get; set; }

        public virtual Company Company { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
