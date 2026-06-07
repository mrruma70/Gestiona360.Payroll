using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.Requests
{
    public class UpdateBranchRequest
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public Guid? ManagerEmployeeId { get; set; }
        public Guid? DefaultCostCenterId { get; set; }
        public bool IsZoneFranca { get; set; }
        public bool IsActive { get; set; }
    }
}
