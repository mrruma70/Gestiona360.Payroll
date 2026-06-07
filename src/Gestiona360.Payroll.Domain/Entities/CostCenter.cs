using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Common;

namespace Gestiona360.Payroll.Domain.Entities
{
    public class CostCenter : BaseEntityGuid
    {
        [Required, MaxLength(20)]
        public string Code { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public Guid? ParentCostCenterId { get; set; }  // ← única FK para el padre
        public Guid? ManagerId { get; set; }
        [MaxLength(30)]
        public string CostType { get; set; }
        public Guid? BranchId { get; set; }
        public bool IsActive { get; set; }

        // Navegación
        public virtual CostCenter Parent { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual ICollection<CostCenter> Children { get; set; }
        public virtual ICollection<PayrollConceptGLMapping> Mappings { get; set; }
    }
}
