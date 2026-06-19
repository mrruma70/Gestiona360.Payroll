using System;
using System.Collections.Generic;
using System.Text;

namespace Gestiona360.Payroll.Domain.Shared.Frontend
{
    /// <summary>
    /// Información de sucursal con detalles del manager y centro de costo.
    /// </summary>
    public class BranchWithDetailsInfo
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public Guid CompanyId { get; set; }
        public bool IsActive { get; set; }
        public bool IsZoneFranca { get; set; }

        // Manager
        public Guid? ManagerEmployeeId { get; set; }
        public string? ManagerFirstName { get; set; }
        public string? ManagerLastName { get; set; }

        // Centro de Costo
        public Guid? DefaultCostCenterId { get; set; }
        public string? CostCenterName { get; set; }
    }
}
