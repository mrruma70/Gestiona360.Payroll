using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Common;

namespace Gestiona360.Payroll.Domain.Entities
{
    public class AttendanceRecord : BaseEntityGuid
    {
        public Guid EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan? CheckIn { get; set; }
        public TimeSpan? CheckOut { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal TotalHours { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal OvertimeHours { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal NightHours { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal HolidayHours { get; set; }

        public bool IsVacationDay { get; set; }
        public bool IsSickLeave { get; set; }
        public bool IsJustifiedAbsence { get; set; }

        public virtual Employee Employee { get; set; }
    }
}
