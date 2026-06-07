using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs
{
    public class BranchDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public Guid CompanyId { get; set; }
        public Guid? ManagerEmployeeId { get; set; }
        public string? ManagerName { get; set; } // Para mostrar en UI
        public Guid? DefaultCostCenterId { get; set; }
        public string? CostCenterName { get; set; } // Para mostrar en UI
        public bool IsZoneFranca { get; set; }
        public bool IsActive { get; set; }

        public string? ManagerEmployeeName { get; set; }

    }
}
