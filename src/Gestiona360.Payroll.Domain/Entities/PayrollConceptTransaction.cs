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
    public class PayrollConceptTransaction : BaseEntityGuid
    {
        public Guid PayrollRecordId { get; set; }
        public Guid? EmployeeConceptSettingId { get; set; }
        public Guid PayrollConceptId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? BaseAmount { get; set; }

        [Column(TypeName = "decimal(9,6)")]
        public decimal? Rate { get; set; }

        public bool IsManualAdjustment { get; set; }
        public string Justification { get; set; }
        public DateTime AppliedDate { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Applied";    // Pending / Applied / Skipped / Reprogrammed

        public virtual PayrollRecord PayrollRecord { get; set; }
        public virtual EmployeeConceptSetting EmployeeConceptSetting { get; set; }
        public virtual PayrollConcept PayrollConcept { get; set; }
    }
}
