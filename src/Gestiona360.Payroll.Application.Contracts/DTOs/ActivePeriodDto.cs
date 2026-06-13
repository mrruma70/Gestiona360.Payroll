using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs
{
    public class ActivePeriodDto
    {
        public string Name { get; set; } = string.Empty;
        public int Sequence { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsOpen { get; set; }
    }
}
