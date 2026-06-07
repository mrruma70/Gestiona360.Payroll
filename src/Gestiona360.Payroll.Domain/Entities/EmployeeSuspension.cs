using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Common;
using Gestiona360.Payroll.Domain.Shared.Frontend.Enums;

namespace Gestiona360.Payroll.Domain.Entities
{
    public class EmployeeSuspension : BaseEntityGuid
    {
   
        public Guid EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // Causal legal obligatoria
        public SuspensionType SuspensionType { get; set; }
        public string Justification { get; set; }

        // Obligatorio si SuspensionType == MassForceMajeure
        public string? MitrabAuthorizationNumber { get; set; }


        // 🔗 Vinculación con la acción de personal que la originó (Trazabilidad)
        public Guid? LinkedPersonalActionId { get; set; }
        public virtual PersonalAction? LinkedPersonalAction { get; set; }
   
    }
}
