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
    public class JobGrade : BaseEntityGuid
    {
        public Guid JobPositionId { get; set; }

        [Required, MaxLength(20)]
        public string Code { get; set; }            // SOLD-A

        [Required, MaxLength(100)]
        public string Name { get; set; }            // Soldador A

        [Column(TypeName = "decimal(10,2)")]
        public decimal BaseSalaryMultiplier { get; set; } = 1.0m;

        public bool RequiresLicense { get; set; }
        [MaxLength(100)]
        public string LicenseName { get; set; }
        public bool IsActive { get; set; }

        public virtual JobPosition JobPosition { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
