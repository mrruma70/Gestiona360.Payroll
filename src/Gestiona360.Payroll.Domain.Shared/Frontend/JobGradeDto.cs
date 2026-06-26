using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    public class JobGradeDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string JobPositionName { get; set; } = string.Empty;
        public Guid JobPositionId { get; set; }
        public decimal BaseSalaryMultiplier { get; set; }
        public decimal MinWageNIO { get; set; }
        public string OccupationalRiskName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
