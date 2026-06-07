using System.ComponentModel.DataAnnotations;

namespace Gestiona360.Payroll.Application.Contracts.Requests;

public class CreateEmployeeRequest
{
    // Paso 1: Datos Básicos
    [Required]
    public Guid CompanyId { get; set; }

    [Required]
    public Guid BranchId { get; set; }

    [Required]
    [RegularExpression(@"^\d{3}-\d{6}-\d{4}[A-Z0-9]$", ErrorMessage = "Formato de cédula inválido")]
    public string Identification { get; set; } = string.Empty;

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

    [Required(ErrorMessage = "La fecha de ingreso es requerida")]
    public DateTime? HireDate { get; set; }

    // Paso 2: Contrato & Puesto
    [Required]
    public int ContractTypeId { get; set; }

    [Required]
    public Guid JobGradeId { get; set; }

    [Required]
    [Range(0, 10000000)]
    public decimal BaseSalary { get; set; }

    public Guid? HealthProviderId { get; set; }

    // Paso 3: Bancario
    public int? BankId { get; set; }

    [MaxLength(30)]
    public string BankAccountNumber { get; set; } = string.Empty;

    [MaxLength(30)]
    public string BankAccountType { get; set; } = string.Empty;

    // ✅ DATOS FISCALES
    [MaxLength(20)]
    public string NORUC { get; set; } = string.Empty;

    [MaxLength(20)]
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

    public Guid? CostCenterId { get; set; }
}