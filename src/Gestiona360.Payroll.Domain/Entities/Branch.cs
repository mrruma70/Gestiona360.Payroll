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
    public class Branch : BaseEntityGuid
    {
        [Required, MaxLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Address { get; set; } = string.Empty;

        [MaxLength(50)]
        public string City { get; set; } = string.Empty;

        [Phone, MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        public Guid CompanyId { get; set; }
        public bool IsActive { get; set; } = true;

        // --- NUEVOS CAMPOS (SQL ALIGNMENT) ---
        public Guid? ManagerEmployeeId { get; set; }        // 👤 Gerente Sucursal
        public Guid? DefaultCostCenterId { get; set; }      // 💰 Centro de Costo Default
        public bool IsZoneFranca { get; set; } = false;     // 🏭 Flag Zona Franca

        // --- NAVIGATION ---
        public virtual Company Company { get; set; }

        // Relación con el Gerente asignado (FK_Branches_Employees_ManagerEmployeeId)
        [ForeignKey(nameof(ManagerEmployeeId))]
        public virtual Employee? Manager { get; set; }

        // Relación con el Centro de Costo (FK_Branches_CostCenters_DefaultCostCenterId)
        [ForeignKey(nameof(DefaultCostCenterId))]
        public virtual CostCenter? DefaultCostCenter { get; set; }

        // Relaciones existentes
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public virtual ICollection<CostCenter> CostCenters { get; set; } = new List<CostCenter>();
    }
}
