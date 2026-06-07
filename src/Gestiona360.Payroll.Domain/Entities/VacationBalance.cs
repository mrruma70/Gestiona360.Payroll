using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Common;

namespace Gestiona360.Payroll.Domain.Entities
{
    public class VacationBalance : BaseEntityGuid
    {
        public Guid EmployeeId { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal AccruedDays { get; set; } = 0;

        [Column(TypeName = "decimal(6,2)")]
        public decimal UsedDays { get; set; } = 0;

        [Column(TypeName = "decimal(6,2)")]
        public decimal AvailableDays { get; set; } = 0;

        [Column(TypeName = "decimal(6,2)")]
        public decimal ExpiredDays { get; set; } = 0;

        public DateTime LastAccrualDate { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal AccrualRateDaysPerSemester { get; set; } = 15;  // 15 días cada 6 meses

        public virtual Employee Employee { get; set; }
    }
}
