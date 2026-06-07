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
    public class Shift : BaseEntityGuid
    {
        [Required, MaxLength(50)]
        public string Name { get; set; }

        [Required, MaxLength(20)]
        public string ShiftType { get; set; }       // Matutino / Nocturno / Rotativo

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal TotalHours { get; set; }

        public bool IsNightShift { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<Schedule> Schedules { get; set; }
        public virtual ICollection<EmployeeShiftAssignment> Assignments { get; set; }
    }
}
