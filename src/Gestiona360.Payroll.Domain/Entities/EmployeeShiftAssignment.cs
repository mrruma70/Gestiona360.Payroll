using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Common;

namespace Gestiona360.Payroll.Domain.Entities
{
    public class EmployeeShiftAssignment : BaseEntityGuid
    {
        public Guid EmployeeId { get; set; }
        public Guid ShiftId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Justification { get; set; }
        public Guid? LinkedToPersonalActionId { get; set; }

        public virtual Employee Employee { get; set; }
        public virtual Shift Shift { get; set; }
        public virtual PersonalAction PersonalAction { get; set; }
    }
}
