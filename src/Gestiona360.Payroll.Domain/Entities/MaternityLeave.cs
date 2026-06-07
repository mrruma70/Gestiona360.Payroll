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
    public class MaternityLeave : BaseEntityGuid
    {
        public Guid EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public int DaysTotal { get; set; } = 90;          // 4 pre + 9 postnatal

        [Column(TypeName = "decimal(18,2)")]
        public decimal INSSSubsidyAmount { get; set; }    // 60%

        [Column(TypeName = "decimal(18,2)")]
        public decimal EmployerComplementAmount { get; set; } // 40%

        [MaxLength(20)]
        public string INSSSubsidyStatus { get; set; } = "Pending";

        [MaxLength(20)]
        public string EmployerComplementStatus { get; set; } = "Pending";

        public virtual Employee Employee { get; set; }
    }
}
