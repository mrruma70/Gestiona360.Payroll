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
    public class Garnishment : BaseEntityGuid
    {
        public Guid EmployeeId { get; set; }

        [Required, MaxLength(20)]
        public string Type { get; set; }                  // Alimony / Civil

        public int PercentageLimit { get; set; }          // 25 o 50

        [MaxLength(100)]
        public string BeneficiaryAccount { get; set; }

        [Required, MaxLength(50)]
        public string CourtOrderNumber { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalAmountToWithhold { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal WithheldToDate { get; set; } = 0;

        public virtual Employee Employee { get; set; }
    }
}
