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
        public string? SecondName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string? SecondLastName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? MobilePhone { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Gender { get; set; }
        public string? MaritalStatus { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime? FirstHireDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public bool IsActive { get; set; }
        public string EmploymentStatus { get; set; } = string.Empty;
        public string? Notes { get; set; }

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
        public int? OccupationalRiskId { get; set; }
        public Guid PayrollGroupId { get; set; }
        public int? DepartmentId { get; set; }
        public int? MunicipalityId { get; set; }

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
        public string PayrollGroupName { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
        public string? MunicipalityName { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // DATOS FINANCIEROS
        // ═══════════════════════════════════════════════════════════════
        public decimal BaseSalary { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankAccountType { get; set; }
        public string? BankBeneficiaryName { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // IMÁGENES / DOCUMENTOS DEL EMPLEADO
        // ═══════════════════════════════════════════════════════════════
        public string? PhotoUrl { get; set; }
        public string? IdFrontUrl { get; set; }
        public string? IdBackUrl { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // DATOS FISCALES
        // ═══════════════════════════════════════════════════════════════
        public string? NORUC { get; set; }
        public string? NOINSS { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // CONDICIONES ESPECIALES
        // ═══════════════════════════════════════════════════════════════
        public bool IsTrustEmployee { get; set; }
        public bool UsesTimeClock { get; set; }
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
        public bool IsOnProbation { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // SUSPENSIÓN
        // ═══════════════════════════════════════════════════════════════
        public DateTime? SuspensionStartDate { get; set; }
        public DateTime? SuspensionEndDate { get; set; }
        public string? SuspensionJustification { get; set; }
        public string? MitrabAuthorizationNumber { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // CONTACTO DE EMERGENCIA
        // ═══════════════════════════════════════════════════════════════
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? EmergencyContactRelationship { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // DOMICILIO
        // ═══════════════════════════════════════════════════════════════
        public string? Address { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // REINGRESO
        // ═══════════════════════════════════════════════════════════════
        public Guid? PreviousEmployeeId { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // TURNO ACTUAL
        // ═══════════════════════════════════════════════════════════════
        public string? CurrentShiftName { get; set; }
        public string? CurrentShiftSchedule { get; set; }
        public DateTime? ShiftAssignmentStartDate { get; set; }
        public TimeSpan? ShiftStartTime { get; set; }
        public TimeSpan? ShiftEndTime { get; set; }

        // ═══════════════════════════════════════════════════════════════
        // PROPIEDADES CALCULADAS
        // ═══════════════════════════════════════════════════════════════
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

        // ═══════════════════════════════════════════════════════════════
        // MÉTODOS DE LÓGICA DE PRESENTACIÓN
        // ═══════════════════════════════════════════════════════════════
        public void CalculateProbationStatus()
        {
            var now = DateTime.UtcNow;
            IsOnProbation = ProbationStartDate.HasValue &&
                           (!ProbationEndDate.HasValue || now < ProbationEndDate);
        }

        public void FormatShiftSchedule()
        {
            if (ShiftStartTime.HasValue && ShiftEndTime.HasValue)
            {
                CurrentShiftSchedule = $"{ShiftStartTime.Value:hh\\:mm} - {ShiftEndTime.Value:hh\\:mm}";
            }
        }
    }
}