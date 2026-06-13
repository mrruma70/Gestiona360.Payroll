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
        // ═══════════════════════════════════════════════════════════════
        // DATOS BÁSICOS
        // ═══════════════════════════════════════════════════════════════
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

        // ═══════════════════════════════════════════════════════════════
        // IDs DE ENTIDADES RELACIONADAS
        // ═══════════════════════════════════════════════════════════════
        public Guid CompanyId { get; set; }
        public Guid BranchId { get; set; }
        public int? ContractTypeId { get; set; }
        public Guid? JobGradeId { get; set; }
        public Guid? HealthProviderId { get; set; }
        public int? BankId { get; set; }
        public Guid? CostCenterId { get; set; }
        public int? OccupationalRiskId { get; set; } // ✅ AGREGADO
        public Guid PayrollGroupId { get; set; } // ✅ NUEVO

        // ═══════════════════════════════════════════════════════════════
        // NOMBRES DE ENTIDADES RELACIONADAS
        // ═══════════════════════════════════════════════════════════════
        public string CompanyName { get; set; } = string.Empty;
        public string BranchName { get; set; } = string.Empty;
        public string BranchCode { get; set; } = string.Empty;
        public string ContractTypeName { get; set; } = string.Empty;
        public string JobPositionName { get; set; } = string.Empty;
        public string JobGradeName { get; set; } = string.Empty;
        public string HealthProviderName { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string CostCenterName { get; set; } = string.Empty;
        public string CostCenterCode { get; set; } = string.Empty;
        public string OccupationalRiskName { get; set; } = string.Empty;
        public string PayrollGroupName { get; set; } = string.Empty; // ✅ NUEVO

        // ═══════════════════════════════════════════════════════════════
        // DATOS FINANCIEROS
        // ═══════════════════════════════════════════════════════════════
        public decimal BaseSalary { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankAccountType { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // IMÁGENES / DOCUMENTOS DEL EMPLEADO
        // ═══════════════════════════════════════════════════════════════
        public string? PhotoUrl { get; set; } // ✅ AGREGADO
        public string? IdFrontUrl { get; set; } // ✅ AGREGADO
        public string? IdBackUrl { get; set; } // ✅ AGREGADO

        // ═══════════════════════════════════════════════════════════════
        // DATOS FISCALES
        // ═══════════════════════════════════════════════════════════════
        public string? NORUC { get; set; }
        public string? NOINSS { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // CONDICIONES ESPECIALES
        // ═══════════════════════════════════════════════════════════════
        public bool IsTrustEmployee { get; set; }
        public decimal? BenefitsInKindValue { get; set; }
        public string? BenefitsInKindDescription { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // TRABAJADOR EXTRANJERO
        // ═══════════════════════════════════════════════════════════════
        public string? Nationality { get; set; }
        public string? WorkPermitNumber { get; set; }
        public DateTime? WorkPermitExpirationDate { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // PERÍODO DE PRUEBA
        // ═══════════════════════════════════════════════════════════════
        public DateTime? ProbationStartDate { get; set; }
        public DateTime? ProbationEndDate { get; set; }
        public bool IsOnProbation { get; set; } // ✅ AGREGADO

        // ═══════════════════════════════════════════════════════════════
        // ESTADO (como string para el frontend)
        // ═══════════════════════════════════════════════════════════════
        public string EmploymentStatus { get; set; } = string.Empty; // ✅ CAMBIADO a string

        // ═══════════════════════════════════════════════════════════════
        // SUSPENSIÓN
        // ═══════════════════════════════════════════════════════════════
        public DateTime? SuspensionStartDate { get; set; } // ✅ AGREGADO
        public DateTime? SuspensionEndDate { get; set; } // ✅ AGREGADO
        public string? SuspensionJustification { get; set; } // ✅ AGREGADO
        public string? MitrabAuthorizationNumber { get; set; } // ✅ AGREGADO

        // ═══════════════════════════════════════════════════════════════
        // TURNO ACTUAL
        // ═══════════════════════════════════════════════════════════════
        public string? CurrentShiftName { get; set; } // ✅ AGREGADO
        public string? CurrentShiftSchedule { get; set; } // ✅ AGREGADO
        public DateTime? ShiftAssignmentStartDate { get; set; } // ✅ AGREGADO

        // ═══════════════════════════════════════════════════════════════
        // NOTAS Y REINGRESO
        // ═══════════════════════════════════════════════════════════════
        public string? Notes { get; set; }
        public Guid? PreviousEmployeeId { get; set; }

        // ✅ NUEVOS: Nombres completos
        public string? SecondName { get; set; }
        public string? SecondLastName { get; set; }

        // ✅ NUEVOS: Datos personales
        public DateTime? BirthDate { get; set; }
        public string? Gender { get; set; }
        public string? MaritalStatus { get; set; }

        // Calculados
        public int? Age => BirthDate.HasValue
            ? (int)(DateTime.UtcNow - BirthDate.Value).TotalDays / 365
            : null;
        public string? GenderText => Gender == "M" ? "Masculino" : Gender == "F" ? "Femenino" : null;
        public string? MaritalStatusText => MaritalStatus switch
        {
            "S" => "Soltero/a",
            "C" => "Casado/a",
            "D" => "Divorciado/a",
            "V" => "Viudo/a",
            "U" => "Unión libre",
            _ => null
        };

        // ✅ NUEVOS: Domicilio
        public string? Address { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int? MunicipalityId { get; set; }
        public string? MunicipalityName { get; set; }

        // ✅ NUEVO: Celular
        public string? MobilePhone { get; set; }

        // ✅ NUEVOS: Contacto de emergencia
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? EmergencyContactRelationship { get; set; }

        // ✅ NUEVO: Beneficiario bancario
        public string? BankBeneficiaryName { get; set; }

        // ✅ NUEVO: Fecha primer ingreso
        public DateTime? FirstHireDate { get; set; }

        // ✅ NUEVO: Reloj de marcas
        public bool UsesTimeClock { get; set; }
    }
}
