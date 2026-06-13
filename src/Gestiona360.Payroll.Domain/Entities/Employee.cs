using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gestiona360.Payroll.Domain.Common;
using Gestiona360.Payroll.Domain.Shared.Frontend;

namespace Gestiona360.Payroll.Domain.Entities
{
    public class Employee : BaseEntityGuid
    {
        // ═══════════════════════════════════════════════════════════════
        // IDENTIFICACIÓN BÁSICA
        // ═══════════════════════════════════════════════════════════════
        [MaxLength(20)]
        public string Code { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Identification { get; set; } = string.Empty;

        // ✅ NUEVOS: Nombres completos (estándar nicaragüense)
        [Required, MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? SecondName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? SecondLastName { get; set; }

        // ✅ NUEVO: Nombre completo calculado
        [NotMapped]
        public string FullName => $"{FirstName} {SecondName} {LastName} {SecondLastName}"
            .Replace("  ", " ").Trim();

        // ═══════════════════════════════════════════════════════════════
        // DATOS PERSONALES (NUEVOS)
        // ═══════════════════════════════════════════════════════════════
        public DateTime? BirthDate { get; set; }

        [MaxLength(1)]
        public string? Gender { get; set; } // "M" o "F"

        [MaxLength(20)]
        public string? MaritalStatus { get; set; } // "S"oltero, "C"asado, "D"ivorciado, "V"iudo, "U"nión libre

        // ═══════════════════════════════════════════════════════════════
        // CONTACTO
        // ═══════════════════════════════════════════════════════════════
        [EmailAddress, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Phone, MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Phone, MaxLength(20)]
        public string? MobilePhone { get; set; } // ✅ NUEVO: Celular

        // ═══════════════════════════════════════════════════════════════
        // DOMICILIO (NUEVO)
        // ═══════════════════════════════════════════════════════════════
        [MaxLength(300)]
        public string? Address { get; set; }

        public int? DepartmentId { get; set; }
        public int? MunicipalityId { get; set; }
        public int? NeighborhoodId { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // CONTACTO DE EMERGENCIA (NUEVO)
        // ═══════════════════════════════════════════════════════════════
        [MaxLength(100)]
        public string? EmergencyContactName { get; set; }

        [Phone, MaxLength(20)]
        public string? EmergencyContactPhone { get; set; }

        [MaxLength(50)]
        public string? EmergencyContactRelationship { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // DOCUMENTOS LEGALES
        // ═══════════════════════════════════════════════════════════════
        [MaxLength(20)]
        public string NORUC { get; set; } = string.Empty;

        [MaxLength(20)]
        public string NOINSS { get; set; } = string.Empty;

        // ═══════════════════════════════════════════════════════════════
        // CONTRATACIÓN
        // ═══════════════════════════════════════════════════════════════
        public DateTime HireDate { get; set; }
        public DateTime? FirstHireDate { get; set; } // ✅ NUEVO: Fecha primer ingreso (antigüedad real)
        public DateTime? TerminationDate { get; set; }

        public Guid CompanyId { get; set; }
        public Guid BranchId { get; set; }

        public int? ContractTypeId { get; set; }
        public Guid? JobGradeId { get; set; }
        public Guid? HealthProviderId { get; set; }
        public int? OccupationalRiskId { get; set; }

        public bool IsActive { get; set; } = true;

        // ═══════════════════════════════════════════════════════════════
        // DATOS BANCARIOS
        // ═══════════════════════════════════════════════════════════════
        public int? BankId { get; set; }

        [MaxLength(30)]
        public string? BankAccountNumber { get; set; }

        [MaxLength(30)]
        public string? BankAccountType { get; set; }

        [MaxLength(100)]
        public string? BankBeneficiaryName { get; set; } // ✅ NUEVO

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal BaseSalary { get; set; } = 0;

        // ═══════════════════════════════════════════════════════════════
        // IMÁGENES
        // ═══════════════════════════════════════════════════════════════
        public string? PhotoUrl { get; set; }
        public string? IdFrontUrl { get; set; }
        public string? IdBackUrl { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // ESTADO LABORAL
        // ═══════════════════════════════════════════════════════════════
        public EmploymentStatus EmploymentStatus { get; set; }

        public DateTime? SuspensionStartDate { get; set; }
        public DateTime? SuspensionEndDate { get; set; }
        public string? SuspensionJustification { get; set; }
        public string? MitrabAuthorizationNumber { get; set; }

        public Guid? PreviousEmployeeId { get; set; }

        public DateTime? ProbationStartDate { get; set; }
        public DateTime? ProbationEndDate { get; set; }

        public bool IsTrustEmployee { get; set; }
        public bool UsesTimeClock { get; set; } // ✅ NUEVO: Usa reloj de marcas

        // ═══════════════════════════════════════════════════════════════
        // TRABAJADOR EXTRANJERO
        // ═══════════════════════════════════════════════════════════════
        public string? Nationality { get; set; }
        public string? WorkPermitNumber { get; set; }
        public DateTime? WorkPermitExpirationDate { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // BENEFICIOS EN ESPECIE
        // ═══════════════════════════════════════════════════════════════
        public decimal? BenefitsInKindValue { get; set; }
        public string? BenefitsInKindDescription { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // NOTAS
        // ═══════════════════════════════════════════════════════════════
        public string? Notes { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // ASIGNACIONES
        // ═══════════════════════════════════════════════════════════════
        public Guid? CostCenterId { get; set; }
        public Guid PayrollGroupId { get; set; }

        [MaxLength(50)]
        public string? CodigoBarra { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // NAVEGACIÓN
        // ═══════════════════════════════════════════════════════════════
        public virtual Company Company { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual ContractType ContractType { get; set; }
        public virtual JobGrade JobGrade { get; set; }
        public virtual HealthProvider HealthProvider { get; set; }
        public virtual OccupationalRisk OccupationalRisk { get; set; }
        public virtual Bank? Bank { get; set; }
        public virtual CostCenter? CostCenter { get; set; }
        public virtual PayrollGroup? PayrollGroup { get; set; }

        // ✅ NUEVAS NAVEGACIONES
        public virtual Department? Department { get; set; }
        public virtual Municipality? Municipality { get; set; }
        public virtual Neighborhood? Neighborhood { get; set; }

        public virtual ICollection<EmployeeShiftAssignment> ShiftAssignments { get; set; }
        public virtual ICollection<EmployeeConceptSetting> ConceptSettings { get; set; }
        public virtual ICollection<Garnishment> Garnishments { get; set; }
        public virtual ICollection<Termination> Terminations { get; set; }
        public virtual ICollection<AttendanceRecord> AttendanceRecords { get; set; }
        public virtual ICollection<PersonalAction> PersonalActions { get; set; }
        public virtual ICollection<VacationBalance> VacationBalances { get; set; }
        public virtual ICollection<ThirteenthMonth> ThirteenthMonths { get; set; }
        public virtual ICollection<IndemnityProvision> IndemnityProvisions { get; set; }
        public virtual ICollection<MaternityLeave> MaternityLeaves { get; set; }
        public virtual ICollection<PayrollRecord> PayrollRecords { get; set; }
    }
}
