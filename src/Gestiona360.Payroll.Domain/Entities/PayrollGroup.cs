using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Common;

namespace Gestiona360.Payroll.Domain.Entities
{
    public class PayrollGroup : BaseEntityGuid
    {
        [Required, MaxLength(50)]
        public string Name { get; set; }

        public string? Code { get; set; }

        public int FrequencyId { get; set; }             // FK a PayrollFrequency (int)
        [MaxLength(20)]
        public string CostCenterCode { get; set; }
        public int ClosingDayRule { get; set; }          // día de cierre
        public DateTime FirstPeriodStartDate { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public virtual PayrollFrequency Frequency { get; set; }
        public virtual ICollection<PayrollPeriod> Periods { get; set; }
    }
}
