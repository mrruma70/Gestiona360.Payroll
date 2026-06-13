using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.PersonalActions
{
    /// <summary>
    /// DTO ligero con los datos actuales del empleado para prellenar el formulario de creación.
    /// </summary>
    public class EmployeeCurrentDataDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;

        // Datos para OldValues
        public decimal BaseSalary { get; set; }
        public Guid? JobGradeId { get; set; }
        public string? JobGradeName { get; set; }

        public int? ContractTypeId { get; set; }
        public string? ContractTypeName { get; set; }

        public Guid? ShiftId { get; set; } // Puede venir de la asignación activa
        public string? ShiftName { get; set; }

        public Guid? CostCenterId { get; set; }
        public string? CostCenterCode { get; set; }

        public DateTime HireDate { get; set; }
        public int EmploymentStatus { get; set; } // Enum como int
    }
}
