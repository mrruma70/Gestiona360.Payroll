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
    public class CurrencyExchange : BaseEntityGuid
    {
        public DateTime Date { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal BCNRate { get; set; }       // Tasa de cambio oficial
        [MaxLength(20)]
        public string Source { get; set; }         // API / Manual
    }
}
