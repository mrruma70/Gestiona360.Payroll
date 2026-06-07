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
    public class ThirteenthMonth : BaseEntityGuid
    {
        public Guid EmployeeId { get; set; }
        public int Year { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Provision { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal FinalCalculationAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CalculationBaseAmount { get; set; } // último salario o máximo de 6 meses

        public DateTime? PaymentDate { get; set; }

        [MaxLength(20)]
        public string PaidStatus { get; set; } = "Pending";

        public virtual Employee Employee { get; set; }
    }
}
