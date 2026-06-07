using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.Requests
{
    public class UpdateEmployeeRequest
    {
        [Required]
        public Guid Id { get; set; }

        // Datos básicos (editables)
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        // Asignación laboral
        [Required]
        public Guid BranchId { get; set; }

        [Required]
        public int ContractTypeId { get; set; }

        [Required]
        public Guid JobGradeId { get; set; }

        [Required]
        [Range(0, 10000000)]
        public decimal BaseSalary { get; set; }

        public Guid? HealthProviderId { get; set; }

        // Datos bancarios
        public int? BankId { get; set; }

        [MaxLength(30)]
        public string BankAccountNumber { get; set; } = string.Empty;

        [MaxLength(30)]
        public string BankAccountType { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public string? PhotoUrl { get; set; }
        public string? IdFrontUrl { get; set; }
        public string? IdBackUrl { get; set; }

        // ✅ DATOS FISCALES
        [Required, MaxLength(20)]
        public string NORUC { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string NOINSS { get; set; } = string.Empty;

        // ✅ CONDICIONES ESPECIALES
        public bool IsTrustEmployee { get; set; }

        // ✅ TRABAJADOR EXTRANJERO
        public string? Nationality { get; set; }
        public string? WorkPermitNumber { get; set; }
        public DateTime? WorkPermitExpirationDate { get; set; }

        // ✅ SALARIO EN ESPECIE
        public decimal? BenefitsInKindValue { get; set; }
        public string? BenefitsInKindDescription { get; set; }

        // ✅ NOTAS
        public string? Notes { get; set; }

        // ⚠️ SOLO LECTURA (calculados por el sistema)
        public DateTime? ProbationStartDate { get; set; }
        public DateTime? ProbationEndDate { get; set; }

        public Guid? CostCenterId { get; set; }
    }
}
