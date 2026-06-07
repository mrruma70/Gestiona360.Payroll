using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Common;

namespace Gestiona360.Payroll.Domain.Entities
{
    public class PayrollConceptGLMapping : BaseEntityGuid
    {
        public Guid PayrollConceptId { get; set; }
        public Guid CostCenterId { get; set; }
        public Guid? JobGradeId { get; set; }
        public Guid GLAccountId { get; set; }
        public bool IsActive { get; set; }

        public virtual PayrollConcept PayrollConcept { get; set; }
        public virtual CostCenter CostCenter { get; set; }
        public virtual JobGrade JobGrade { get; set; }
        public virtual GLAccount GLAccount { get; set; }
    }
}
