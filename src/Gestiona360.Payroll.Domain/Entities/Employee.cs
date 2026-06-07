using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Common;
using Gestiona360.Payroll.Domain.Shared.Frontend;

namespace Gestiona360.Payroll.Domain.Entities
{
    public class Employee : BaseEntityGuid
    {

        [MaxLength(20)]
        public string Code { get; set; }

        [Required, MaxLength(20)]
        public string Identification { get; set; }   // Cédula

        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        [EmailAddress, MaxLength(100)]
        public string Email { get; set; }

        [Phone, MaxLength(20)]
        public string Phone { get; set; }

        public DateTime HireDate { get; set; }
        public DateTime? TerminationDate { get; set; }

        public Guid CompanyId { get; set; }
        public Guid BranchId { get; set; }

        public int? ContractTypeId { get; set; }
        public Guid? JobGradeId { get; set; }
        public Guid? HealthProviderId { get; set; }
        public int? OccupationalRiskId { get; set; }

        public bool IsActive { get; set; } = true;

        public int? BankId { get; set; }                   // FK a Bank
        [MaxLength(30)]
        public string? BankAccountNumber { get; set; }     // Número de cuenta
        [MaxLength(30)]
        public string? BankAccountType { get; set; }       // "Ahorro", "Corriente"

        [Required]      
        [Column(TypeName = "decimal(18,2)")]
        public decimal BaseSalary { get; set; } = 0;

        // Imagenes
        public string? PhotoUrl { get; set; }
        public string? IdFrontUrl { get; set; }
        public string? IdBackUrl { get; set; }

        [MaxLength(20)]
        public string NORUC { get; set; }   // RUC del empleado (si aplica)

        [MaxLength(20)]
        public string NOINSS { get; set; }   // INSS del empleado (si aplica)


        // Estado detallado del empleado (más semántico que solo IsActive)
        public EmploymentStatus EmploymentStatus { get; set; }  // Active, Suspended, Terminated

        // Suspensión temporal
        public DateTime? SuspensionStartDate { get; set; }
        public DateTime? SuspensionEndDate { get; set; }
        public string? SuspensionJustification { get; set; }
        public string? MitrabAuthorizationNumber { get; set; }  // Para suspensiones colectivas

        // Reingreso (vínculo con registro anterior)
        public Guid? PreviousEmployeeId { get; set; }  // FK a la misma tabla Employees

        // Período de prueba segun Art. 44 inciso 2 (máximo 3 meses, prorrogable por 1 mes adicional)   
        // en algunas como zona franca se deja una semana antes de tomarla como fecha de inicio de contrato,
        // para que el empleado pueda realizar trámites de documentación y demás requisitos previos al inicio formal del contrato.
        public DateTime? ProbationStartDate { get; set; }
        public DateTime? ProbationEndDate { get; set; }
        public bool IsOnProbation => ProbationStartDate.HasValue &&
                                      !ProbationEndDate.HasValue &&
                                      DateTime.UtcNow < ProbationEndDate;

        // Empleado de confianza (Art. 44 inciso 3)
        public bool IsTrustEmployee { get; set; }

        // Trabajador extranjero
        public string? Nationality { get; set; }
        public string? WorkPermitNumber { get; set; }
        public DateTime? WorkPermitExpirationDate { get; set; }

        // Salario en especie (beneficios que constituyen salario)
        public decimal? BenefitsInKindValue { get; set; }  // Valor mensual estimado
        public string? BenefitsInKindDescription { get; set; }

        // Notas adicionales
        public string? Notes { get; set; }

        public Guid? CostCenterId { get; set; }

        // Navigation
        public virtual Company Company { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual ContractType ContractType { get; set; }
        public virtual JobGrade JobGrade { get; set; }
        public virtual HealthProvider HealthProvider { get; set; }
        public virtual OccupationalRisk OccupationalRisk { get; set; }
        public virtual Bank? Bank { get; set; }
        public virtual CostCenter? CostCenter { get; set; }
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
