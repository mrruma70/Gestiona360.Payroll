using System;
using System.ComponentModel.DataAnnotations;

namespace Gestiona360.Payroll.Application.Contracts.Requests
{
    public class CreateEmployeeRequest
    {
        // ═══════════════════════════════════════════════════════════════
        // IDENTIFICACIÓN BÁSICA
        // ═══════════════════════════════════════════════════════════════
        [Required(ErrorMessage = "La cédula es obligatoria")]
        [MaxLength(20)]
        public string Identification { get; set; } = string.Empty;

        [Required(ErrorMessage = "El primer nombre es obligatorio")]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? SecondName { get; set; }

        [Required(ErrorMessage = "El primer apellido es obligatorio")]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? SecondLastName { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // DATOS DEMOGRÁFICOS (OBLIGATORIOS)
        // ═══════════════════════════════════════════════════════════════
        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        public DateTime? BirthDate { get; set; }

        [Required(ErrorMessage = "El sexo es obligatorio")]
        [MaxLength(1)]
        public string? Gender { get; set; }

        [Required(ErrorMessage = "El estado civil es obligatorio")]
        [MaxLength(20)]
        public string? MaritalStatus { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // CONTACTO (CELULAR OBLIGATORIO)
        // ═══════════════════════════════════════════════════════════════
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "El celular es obligatorio")]
        [MaxLength(20)]
        public string? MobilePhone { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // DOMICILIO (OBLIGATORIOS)
        // ═══════════════════════════════════════════════════════════════
        [Required(ErrorMessage = "La dirección es obligatoria")]
        [MaxLength(300)]
        public string? Address { get; set; }

        [Required(ErrorMessage = "El departamento es obligatorio")]
        public int? DepartmentId { get; set; }

        [Required(ErrorMessage = "El municipio es obligatorio")]
        public int? MunicipalityId { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // CONTACTO DE EMERGENCIA (OBLIGATORIOS)
        // ═══════════════════════════════════════════════════════════════
        [Required(ErrorMessage = "El contacto de emergencia es obligatorio")]
        [MaxLength(100)]
        public string? EmergencyContactName { get; set; }

        [Required(ErrorMessage = "El teléfono de emergencia es obligatorio")]
        [MaxLength(20)]
        public string? EmergencyContactPhone { get; set; }

        [Required(ErrorMessage = "El parentesco es obligatorio")]
        [MaxLength(50)]
        public string? EmergencyContactRelationship { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // DATOS FISCALES
        // ═══════════════════════════════════════════════════════════════
        [Required(ErrorMessage = "El RUC es obligatorio")]
        [MaxLength(20)]
        public string NORUC { get; set; } = string.Empty;

        [Required(ErrorMessage = "El INSS es obligatorio")]
        [MaxLength(20)]
        public string NOINSS { get; set; } = string.Empty;

        // ═══════════════════════════════════════════════════════════════
        // CONTRATACIÓN
        // ═══════════════════════════════════════════════════════════════
        [Required(ErrorMessage = "La fecha de ingreso es obligatoria")]
        public DateTime? HireDate { get; set; }

        public DateTime? FirstHireDate { get; set; }

        [Required]
        public Guid CompanyId { get; set; }

        [Required]
        public Guid BranchId { get; set; }

        [Required]
        public int ContractTypeId { get; set; }

        [Required]
        public Guid JobGradeId { get; set; }

        [Required]
        public Guid PayrollGroupId { get; set; }

        [Required, Range(0, 10000000)]
        public decimal BaseSalary { get; set; }

        public Guid? HealthProviderId { get; set; }
        public Guid? CostCenterId { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // DATOS BANCARIOS
        // ═══════════════════════════════════════════════════════════════
        public int? BankId { get; set; }
        [MaxLength(30)]
        public string? BankAccountNumber { get; set; }
        [MaxLength(30)]
        public string? BankAccountType { get; set; }
        [MaxLength(100)]
        public string? BankBeneficiaryName { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // CONDICIONES ESPECIALES
        // ═══════════════════════════════════════════════════════════════
        public bool IsTrustEmployee { get; set; }
        public bool UsesTimeClock { get; set; } = true; // ✅ Por defecto sí usa reloj
        public decimal? BenefitsInKindValue { get; set; }
        public string? BenefitsInKindDescription { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // TRABAJADOR EXTRANJERO
        // ═══════════════════════════════════════════════════════════════
        public string? Nationality { get; set; }
        public string? WorkPermitNumber { get; set; }
        public DateTime? WorkPermitExpirationDate { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // NOTAS
        // ═══════════════════════════════════════════════════════════════
        public string? Notes { get; set; }
    }
}