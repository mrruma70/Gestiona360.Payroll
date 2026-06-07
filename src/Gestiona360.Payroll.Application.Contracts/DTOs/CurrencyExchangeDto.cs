using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs
{
    public class CurrencyExchangeDto
    {
        public DateTime Date { get; set; }
        public decimal BCNRate { get; set; }
        public string Source { get; set; } = string.Empty;
    }
}
