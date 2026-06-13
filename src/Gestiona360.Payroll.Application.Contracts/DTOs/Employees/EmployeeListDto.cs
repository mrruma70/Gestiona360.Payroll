using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Shared.Frontend;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.Employees
{
    public class EmployeeListDto
    {
        // Campos básicos existentes
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Identification { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public bool IsActive { get; set; }

        // Datos relacionados (para mostrar en la tabla)
        public string JobPositionName { get; set; } = string.Empty;
        public string JobGradeName { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string BranchCode { get; set; } = string.Empty;
        public string ContractTypeName { get; set; } = string.Empty;

        // Propiedad calculada para UI
        public string FullName => $"{FirstName} {LastName}";

        // propiedades para indicadores
        public EmploymentStatus EmploymentStatus { get; set; }
        public bool IsOnProbation { get; set; }
        public bool IsTrustEmployee { get; set; }
        public string? Nationality { get; set; }
        public Guid? PreviousEmployeeId { get; set; }

        //  CAMPOS ADICIONALES ÚTILES (que faltaban)
        public bool IsForeignWorker => !string.IsNullOrEmpty(Nationality);
        public bool IsRehire => PreviousEmployeeId.HasValue;
        public DateTime? ProbationEndDate { get; set; }

        public Guid? CostCenterId { get; set; }
        public string CostCenterName { get; set; } = string.Empty;
        public string CostCenterCode { get; set; } = string.Empty;

        public decimal BaseSalary { get; set; }
        public string ShiftName { get; set; } = string.Empty;

        public int? BankId { get; set; }
        public string BankName { get; set; } = string.Empty;
        public string BankAccountNumber { get; set; } = string.Empty;
        public Guid? HealthProviderId { get; set; }
        public string HealthProviderName { get; set; } = string.Empty;

        //  Grupo de Nómina
        public Guid PayrollGroupId { get; set; }
        public string PayrollGroupName { get; set; } = string.Empty;

        // Propiedad calculada para mostrar el estado como texto
        public string EmploymentStatusName => EmploymentStatus switch
        {
            EmploymentStatus.Active => "Activo",
            EmploymentStatus.Suspended => "Suspendido",
            EmploymentStatus.Terminated => "Terminado",
            _ => "Desconocido"
        };


    }
}
