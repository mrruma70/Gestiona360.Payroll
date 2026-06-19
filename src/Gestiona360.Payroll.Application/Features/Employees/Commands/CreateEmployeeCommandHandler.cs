// src/Gestiona360.Payroll.Application/Features/Employees/Commands/CreateEmployeeCommandHandler.cs
using System.ComponentModel.DataAnnotations;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Interfaces;
using Gestiona360.Payroll.Domain.Services;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.Employees.Commands;

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, Guid>
{
    private readonly IEmployeeRepository _employeeRepo;
    private readonly EmployeeDomainService _domainService;

    public CreateEmployeeCommandHandler(
        IEmployeeRepository employeeRepo,
        EmployeeDomainService domainService)
    {
        _employeeRepo = employeeRepo;
        _domainService = domainService;
    }

    public async Task<Guid> Handle(CreateEmployeeCommand command, CancellationToken ct)
    {
        var req = command.Request;

        // 0. Validación de campos obligatorios
        if (req.HireDate is null)
            throw new ValidationException("La fecha de ingreso es obligatoria.");
        if (req.DepartmentId is null || req.MunicipalityId is null)
            throw new ValidationException("Departamento y municipio son obligatorios.");

        // 1. Validaciones de dominio
        var previousId = await _domainService.ValidateIdentificationAndReentryAsync(req.Identification, ct);

        var jobGrade = await _domainService.ValidateJobGradeAndSalaryAsync(
            req.JobGradeId, req.BaseSalary, ct);

        var locationResult = await _domainService.ValidateLocationAsync(
            req.DepartmentId.Value, req.MunicipalityId.Value, ct);

        var probationResult = await _domainService.CalculateProbationPeriodAsync(
            req.ContractTypeId, req.HireDate.Value, ct);

        // 2. Generar código único
        var employeeCode = await _employeeRepo.GenerateNextEmployeeCodeAsync(ct);

        // 3. Construir entidad
        var employee = new Employee
        {
            Code = employeeCode,
            Identification = req.Identification,
            FirstName = req.FirstName,
            SecondName = req.SecondName,
            LastName = req.LastName,
            SecondLastName = req.SecondLastName,
            BirthDate = req.BirthDate,
            Gender = req.Gender,
            MaritalStatus = req.MaritalStatus,
            Email = req.Email,
            Phone = req.Phone,
            MobilePhone = req.MobilePhone,
            Address = req.Address,
            DepartmentId = req.DepartmentId,
            MunicipalityId = req.MunicipalityId,
            EmergencyContactName = req.EmergencyContactName,
            EmergencyContactPhone = req.EmergencyContactPhone,
            EmergencyContactRelationship = req.EmergencyContactRelationship,
            NORUC = req.NORUC,
            NOINSS = req.NOINSS,
            HireDate = req.HireDate.Value,
            FirstHireDate = req.FirstHireDate ?? req.HireDate.Value,
            CompanyId = req.CompanyId,
            BranchId = req.BranchId,
            ContractTypeId = req.ContractTypeId,
            JobGradeId = jobGrade.Id,
            HealthProviderId = req.HealthProviderId,
            OccupationalRiskId = jobGrade.JobPosition?.OccupationalRiskId,
            BaseSalary = req.BaseSalary,
            CostCenterId = req.CostCenterId,
            PayrollGroupId = req.PayrollGroupId,
            BankId = req.BankId,
            BankAccountNumber = req.BankAccountNumber,
            BankAccountType = req.BankAccountType,
            BankBeneficiaryName = req.BankBeneficiaryName,
            IsTrustEmployee = req.IsTrustEmployee,
            UsesTimeClock = req.UsesTimeClock,
            BenefitsInKindValue = req.BenefitsInKindValue,
            BenefitsInKindDescription = req.BenefitsInKindDescription,
            Nationality = req.Nationality,
            WorkPermitNumber = req.WorkPermitNumber,
            WorkPermitExpirationDate = req.WorkPermitExpirationDate,
            EmploymentStatus = EmploymentStatus.Active,
            IsActive = true,
            PreviousEmployeeId = previousId,
            ProbationStartDate = probationResult.Start,   
            ProbationEndDate = probationResult.End, 
            Notes = req.Notes
        };

        // 4. Persistencia
        await _employeeRepo.AddAsync(employee, ct);
        return employee.Id;
    }
}