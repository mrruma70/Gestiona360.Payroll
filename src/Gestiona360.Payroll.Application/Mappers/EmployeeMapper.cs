// src/Gestiona360.Payroll.Application/Mappers/EmployeeMapper.cs

using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Application.Contracts.DTOs.Employees;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Shared.Frontend;

namespace Gestiona360.Payroll.Application.Mappers;

public static class EmployeeMapper
{
    public static EmployeeDetailDto ToDetailDto(Employee e)
    {
        var shiftActual = e.ShiftAssignments?.FirstOrDefault(sa => sa.EndDate == null);

        var dto = new EmployeeDetailDto
        {
            // DATOS BÁSICOS
            Id = e.Id,
            Code = e.Code,
            Identification = e.Identification,
            FirstName = e.FirstName,
            SecondName = e.SecondName,
            LastName = e.LastName,
            SecondLastName = e.SecondLastName,
            BirthDate = e.BirthDate,
            Gender = e.Gender,
            MaritalStatus = e.MaritalStatus,
            Email = e.Email,
            Phone = e.Phone,
            MobilePhone = e.MobilePhone,
            Address = e.Address,
            HireDate = e.HireDate,
            FirstHireDate = e.FirstHireDate,
            TerminationDate = e.TerminationDate,
            IsActive = e.IsActive,
            EmploymentStatus = e.EmploymentStatus.ToString(),
            Notes = e.Notes,

            // IDs
            CompanyId = e.CompanyId,
            BranchId = e.BranchId,
            ContractTypeId = e.ContractTypeId,
            JobGradeId = e.JobGradeId,
            HealthProviderId = e.HealthProviderId,
            BankId = e.BankId,
            CostCenterId = e.CostCenterId,
            OccupationalRiskId = e.OccupationalRiskId,
            PayrollGroupId = e.PayrollGroupId,
            DepartmentId = e.DepartmentId,
            MunicipalityId = e.MunicipalityId,

            // NOMBRES
            CompanyName = e.Company?.LegalName ?? "N/A",
            BranchName = e.Branch?.Name ?? "N/A",
            BranchCode = e.Branch?.Code ?? "N/A",
            ContractTypeName = e.ContractType?.Name ?? "N/A",
            JobPositionName = e.JobGrade?.JobPosition?.Name ?? "N/A",
            JobGradeName = e.JobGrade?.Name ?? "N/A",
            HealthProviderName = e.HealthProvider?.Name ?? "Sin proveedor",
            BankName = e.Bank?.Name ?? "Sin cuenta",
            CostCenterName = e.CostCenter?.Name ?? "No asignado",
            CostCenterCode = e.CostCenter?.Code ?? "",
            OccupationalRiskName = e.OccupationalRisk?.Name ?? "N/A",
            PayrollGroupName = e.PayrollGroup?.Name ?? "No asignado",
            DepartmentName = e.Department?.Name,
            MunicipalityName = e.Municipality?.Name,

            // FINANCIEROS
            BaseSalary = e.BaseSalary,
            BankAccountNumber = e.BankAccountNumber,
            BankAccountType = e.BankAccountType,
            BankBeneficiaryName = e.BankBeneficiaryName,

            // IMÁGENES
            PhotoUrl = e.PhotoUrl,
            IdFrontUrl = e.IdFrontUrl,
            IdBackUrl = e.IdBackUrl,

            // FISCALES
            NORUC = e.NORUC,
            NOINSS = e.NOINSS,

            // CONDICIONES ESPECIALES
            IsTrustEmployee = e.IsTrustEmployee,
            UsesTimeClock = e.UsesTimeClock,
            BenefitsInKindValue = e.BenefitsInKindValue,
            BenefitsInKindDescription = e.BenefitsInKindDescription,

            // EXTRANJERO
            Nationality = e.Nationality,
            WorkPermitNumber = e.WorkPermitNumber,
            WorkPermitExpirationDate = e.WorkPermitExpirationDate,

            // PERÍODO DE PRUEBA
            ProbationStartDate = e.ProbationStartDate,
            ProbationEndDate = e.ProbationEndDate,

            // SUSPENSIÓN
            SuspensionStartDate = e.SuspensionStartDate,
            SuspensionEndDate = e.SuspensionEndDate,
            SuspensionJustification = e.SuspensionJustification,
            MitrabAuthorizationNumber = e.MitrabAuthorizationNumber,

            // CONTACTO DE EMERGENCIA
            EmergencyContactName = e.EmergencyContactName,
            EmergencyContactPhone = e.EmergencyContactPhone,
            EmergencyContactRelationship = e.EmergencyContactRelationship,

            // REINGRESO
            PreviousEmployeeId = e.PreviousEmployeeId,

            // TURNO ACTUAL
            CurrentShiftName = shiftActual?.Shift?.Name,
            ShiftStartTime = shiftActual?.Shift?.StartTime,
            ShiftEndTime = shiftActual?.Shift?.EndTime,
            ShiftAssignmentStartDate = shiftActual?.StartDate
        };

        // ✅ Calcular propiedades derivadas (no se puede hacer en SQL)
        dto.CalculateProbationStatus();
        dto.FormatShiftSchedule();

        return dto;
    }

    public static EmployeeListDto ToListDto(Employee e)
    {
        var shiftActual = e.ShiftAssignments?.FirstOrDefault(sa => sa.EndDate == null);
        var now = DateTime.UtcNow;

        var isOnProbation = e.ProbationStartDate.HasValue &&
                           e.ProbationEndDate.HasValue &&
                           now >= e.ProbationStartDate.Value &&
                           now <= e.ProbationEndDate.Value;

        return new EmployeeListDto
        {
            Id = e.Id,
            Code = e.Code,
            Identification = e.Identification,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Email = e.Email,
            Phone = e.Phone,
            HireDate = e.HireDate,
            IsActive = e.IsActive,

            JobPositionName = e.JobGrade?.JobPosition?.Name ?? "Sin puesto",
            JobGradeName = e.JobGrade?.Name ?? "",
            BranchName = e.Branch?.Name ?? "",
            BranchCode = e.Branch?.Code ?? "",
            ContractTypeName = e.ContractType?.Name ?? "",
            BaseSalary = e.BaseSalary,
            EmploymentStatus = e.EmploymentStatus,
            IsTrustEmployee = e.IsTrustEmployee,
            IsOnProbation = isOnProbation,

            Nationality = e.Nationality,
            ProbationEndDate = e.ProbationEndDate,
            CostCenterId = e.CostCenterId,
            CostCenterName = e.CostCenter?.Name ?? "",
            CostCenterCode = e.CostCenter?.Code ?? "",
            BankId = e.BankId,
            BankName = e.Bank?.Name ?? "",
            BankAccountNumber = e.BankAccountNumber ?? "",
            HealthProviderId = e.HealthProviderId,
            HealthProviderName = e.HealthProvider?.Name ?? "",

            ShiftName = shiftActual?.Shift?.Name ?? "Sin turno asignado",
            PreviousEmployeeId = e.PreviousEmployeeId,
            PayrollGroupId = e.PayrollGroupId,
            PayrollGroupName = e.PayrollGroup?.Name ?? "No asignado"
        };
    }

    public static EmployeeStatsDto ToStatsDto(EmployeeStatsEntity entity)
    {
        return new EmployeeStatsDto
        {
            Total = entity.Total,
            Active = entity.Active,
            Inactive = entity.Inactive,
            NewThisMonth = entity.NewThisMonth,
            Suspended = entity.Suspended,
            Terminated = entity.Terminated,
            OnProbation = entity.OnProbation,
            TrustEmployees = entity.TrustEmployees,
            ForeignWorkers = entity.ForeignWorkers,
            Rehires = entity.Rehires
        };
    }

    /// <summary>
    /// Mapea Employee a EmployeeDetailDto para GetEmployeeByIdQuery.
    /// Maneja la lógica específica de turno actual (EndDate null o futura).
    /// </summary>
    public static EmployeeDetailDto ToDetailDtoWithShift(Employee e)
    {
        // ✅ Obtener turno actual (ya viene filtrado desde el repositorio)
        var currentShiftAssignment = e.ShiftAssignments?
            .OrderByDescending(sa => sa.StartDate)
            .FirstOrDefault();

        var dto = new EmployeeDetailDto
        {
            // DATOS BÁSICOS
            Id = e.Id,
            Code = e.Code,
            Identification = e.Identification,
            FirstName = e.FirstName,
            SecondName = e.SecondName,
            LastName = e.LastName,
            SecondLastName = e.SecondLastName,
            BirthDate = e.BirthDate,
            Gender = e.Gender,
            MaritalStatus = e.MaritalStatus,
            Email = e.Email,
            Phone = e.Phone,
            MobilePhone = e.MobilePhone,
            Address = e.Address,
            HireDate = e.HireDate,
            FirstHireDate = e.FirstHireDate,
            TerminationDate = e.TerminationDate,
            IsActive = e.IsActive,
            EmploymentStatus = e.EmploymentStatus.ToString(),
            Notes = e.Notes,

            // IDs
            CompanyId = e.CompanyId,
            BranchId = e.BranchId,
            ContractTypeId = e.ContractTypeId,
            JobGradeId = e.JobGradeId,
            HealthProviderId = e.HealthProviderId,
            BankId = e.BankId,
            CostCenterId = e.CostCenterId,
            OccupationalRiskId = e.OccupationalRiskId,
            PayrollGroupId = e.PayrollGroupId,
            DepartmentId = e.DepartmentId,
            MunicipalityId = e.MunicipalityId,

            // NOMBRES
            CompanyName = e.Company?.LegalName ?? "N/A",
            BranchName = e.Branch?.Name ?? "N/A",
            BranchCode = e.Branch?.Code ?? "N/A",
            ContractTypeName = e.ContractType?.Name ?? "N/A",
            JobPositionName = e.JobGrade?.JobPosition?.Name ?? "N/A",
            JobGradeName = e.JobGrade?.Name ?? "N/A",
            HealthProviderName = e.HealthProvider?.Name ?? "No asignado",
            BankName = e.Bank?.Name ?? "No asignado",
            CostCenterName = e.CostCenter?.Name ?? "No asignado",
            CostCenterCode = e.CostCenter?.Code ?? "",
            OccupationalRiskName = e.OccupationalRisk?.Name ?? "N/A",
            PayrollGroupName = e.PayrollGroup?.Name ?? "No asignado",
            DepartmentName = e.Department?.Name,
            MunicipalityName = e.Municipality?.Name,

            // FINANCIEROS
            BaseSalary = e.BaseSalary,
            BankAccountNumber = e.BankAccountNumber ?? "N/A",
            BankAccountType = e.BankAccountType ?? "N/A",
            BankBeneficiaryName = e.BankBeneficiaryName,

            // IMÁGENES
            PhotoUrl = e.PhotoUrl,
            IdFrontUrl = e.IdFrontUrl,
            IdBackUrl = e.IdBackUrl,

            // FISCALES
            NORUC = e.NORUC ?? string.Empty,
            NOINSS = e.NOINSS ?? string.Empty,

            // CONDICIONES ESPECIALES
            IsTrustEmployee = e.IsTrustEmployee,
            UsesTimeClock = e.UsesTimeClock,
            BenefitsInKindValue = e.BenefitsInKindValue,
            BenefitsInKindDescription = e.BenefitsInKindDescription,

            // EXTRANJERO
            Nationality = e.Nationality,
            WorkPermitNumber = e.WorkPermitNumber,
            WorkPermitExpirationDate = e.WorkPermitExpirationDate,

            // PERÍODO DE PRUEBA
            ProbationStartDate = e.ProbationStartDate,
            ProbationEndDate = e.ProbationEndDate,

            // SUSPENSIÓN
            SuspensionStartDate = e.SuspensionStartDate,
            SuspensionEndDate = e.SuspensionEndDate,
            SuspensionJustification = e.SuspensionJustification,
            MitrabAuthorizationNumber = e.MitrabAuthorizationNumber,

            // CONTACTO DE EMERGENCIA
            EmergencyContactName = e.EmergencyContactName,
            EmergencyContactPhone = e.EmergencyContactPhone,
            EmergencyContactRelationship = e.EmergencyContactRelationship,

            // REINGRESO
            PreviousEmployeeId = e.PreviousEmployeeId,

            // TURNO ACTUAL (ya viene filtrado)
            CurrentShiftName = currentShiftAssignment?.Shift?.Name,
            ShiftStartTime = currentShiftAssignment?.Shift?.StartTime,
            ShiftEndTime = currentShiftAssignment?.Shift?.EndTime,
            ShiftAssignmentStartDate = currentShiftAssignment?.StartDate
        };

        // Calcular propiedades derivadas
        dto.CalculateProbationStatus();
        dto.FormatShiftSchedule();

        return dto;
    }

    public static EmployeeDto ToDto(EmployeeBasicInfo e)
    {
        return new EmployeeDto
        {
            Id = e.Id,
            FullName = $"{e.FirstName} {e.LastName}",
            Identification = e.Identification,
            Email = e.Email,
            Phone = e.Phone,
            IsActive = e.IsActive
        };
    }
}
