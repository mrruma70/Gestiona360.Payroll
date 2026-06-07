using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Shared.Frontend;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.Employees
{
    public class EmployeeDetailDto
    {
        // === DATOS BÁSICOS ===
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Identification { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public bool IsActive { get; set; }

        // === EMPRESA ===
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;

        // === SUCURSAL ===
        public Guid BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string BranchCode { get; set; } = string.Empty;

        // === CONTRATO ===
        public int ContractTypeId { get; set; }
        public string ContractTypeName { get; set; } = string.Empty;

        // === PUESTO/NIVEL ===
        public Guid? JobGradeId { get; set; }
        public string JobPositionName { get; set; } = string.Empty;
        public string JobGradeName { get; set; } = string.Empty;

        // === SALARIO ===
        public decimal BaseSalary { get; set; }

        // === PROVEEDOR SALUD ===
        public Guid? HealthProviderId { get; set; }
        public string HealthProviderName { get; set; } = string.Empty;

        // === BANCO ===
        public int? BankId { get; set; }
        public string BankName { get; set; } = string.Empty;
        public string? BankAccountNumber { get; set; }
        public string? BankAccountType { get; set; }

        // === RIESGO OCUPACIONAL ===
        public int? OccupationalRiskId { get; set; }
        public string OccupationalRiskName { get; set; } = string.Empty;

        // === DOCUMENTOS ===
        public string? PhotoUrl { get; set; }
        public string? IdFrontUrl { get; set; }
        public string? IdBackUrl { get; set; }

        // === DATOS FISCALES ===
        public string NORUC { get; set; } = string.Empty;
        public string NOINSS { get; set; } = string.Empty;

        // === ESTADO Y SUSPENSIÓN ===
        public EmploymentStatus EmploymentStatus { get; set; }
        public DateTime? SuspensionStartDate { get; set; }
        public DateTime? SuspensionEndDate { get; set; }
        public string? SuspensionJustification { get; set; }
        public string? MitrabAuthorizationNumber { get; set; }

        // === REINGRESO ===
        public Guid? PreviousEmployeeId { get; set; }

        // === PERÍODO DE PRUEBA ===
        public DateTime? ProbationStartDate { get; set; }
        public DateTime? ProbationEndDate { get; set; }
        public bool IsOnProbation { get; set; }  // ✅ Propiedad calculada

        // === CONDICIONES ESPECIALES ===
        public bool IsTrustEmployee { get; set; }
        public decimal? BenefitsInKindValue { get; set; }
        public string? BenefitsInKindDescription { get; set; }

        // === TRABAJADOR EXTRANJERO ===
        public string? Nationality { get; set; }
        public string? WorkPermitNumber { get; set; }
        public DateTime? WorkPermitExpirationDate { get; set; }

        // === NOTAS ===
        public string? Notes { get; set; }

        // === TURNO ACTUAL (NUEVO) ===
        public string? CurrentShiftName { get; set; }
        public string? CurrentShiftSchedule { get; set; }
        public DateTime? ShiftAssignmentStartDate { get; set; }

        public Guid? CostCenterId { get; set; }
        public string CostCenterName { get; set; } = string.Empty;
        public string CostCenterCode { get; set; } = string.Empty;
    }
}
