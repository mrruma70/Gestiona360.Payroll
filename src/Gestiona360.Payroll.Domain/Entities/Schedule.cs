using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Common;

namespace Gestiona360.Payroll.Domain.Entities
{
    public class Schedule : BaseEntityGuid
    {
        public Guid ShiftId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }     // Lunes a Domingo
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsRestDay { get; set; }
        public bool IsActive { get; set; }

        public virtual Shift Shift { get; set; }
    }
}
