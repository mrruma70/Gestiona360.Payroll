// src/Gestiona360.Payroll.Domain/Services/EmployeeDomainService.cs
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Exceptions;
using Gestiona360.Payroll.Domain.Interfaces;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using Gestiona360.Payroll.Domain.Shared.Validation;

namespace Gestiona360.Payroll.Domain.Services;

/// <summary>
/// Servicio de dominio que encapsula TODAS las reglas de negocio para empleados.
/// Testeable unitariamente sin base de datos mediante mocks de repositorios.
/// Reemplaza completamente las validaciones inline de Create/Update handlers originales.
/// </summary>
public class EmployeeDomainService
{
    private readonly IEmployeeRepository _employeeRepo;
    private readonly IJobGradeRepository _jobGradeRepo;
    private readonly IDepartmentRepository _departmentRepo;
    private readonly IMunicipalityRepository _municipalityRepo;
    private readonly IContractTypeRepository _contractTypeRepo;
    private readonly IMinimumWageRepository _minimumWageRepo;

    public EmployeeDomainService(
        IEmployeeRepository employeeRepo,
        IJobGradeRepository jobGradeRepo,
        IDepartmentRepository departmentRepo,
        IMunicipalityRepository municipalityRepo,
        IContractTypeRepository contractTypeRepo,
        IMinimumWageRepository minimumWageRepo)
    {
        _employeeRepo = employeeRepo;
        _jobGradeRepo = jobGradeRepo;
        _departmentRepo = departmentRepo;
        _municipalityRepo = municipalityRepo;
        _contractTypeRepo = contractTypeRepo;
        _minimumWageRepo = minimumWageRepo;
    }

    /// <summary>
    /// Valida cédula nicaragüense, unicidad y reingreso SOLO si estado es Terminated.
    /// Usado EXCLUSIVAMENTE por CreateEmployeeCommandHandler.
    /// Retorna PreviousEmployeeId para vincular historial, o null si es creación nueva.
    /// </summary>
    public async Task<Guid?> ValidateIdentificationAndReentryAsync(
        string identification, CancellationToken ct)
    {
        if (!CedulaValidator.EsValida(identification))
            throw new BusinessRuleViolationException(
                "La cédula ingresada no es válida según el algoritmo de verificación nicaragüense.");

        var existing = await _employeeRepo.GetByIdentificationAsync(identification, ct);

        // Si NO existe → creación nueva
        if (existing == null)
            return null;

        // SI existe → solo permitir reingreso si está TERMINADO
        if (existing.EmploymentStatus != EmploymentStatus.Terminated)
        {
            throw new BusinessRuleViolationException(
                $"Ya existe un empleado activo/suspendido con la cédula {identification}. " +
                $"Estado actual: {existing.EmploymentStatus}. " +
                "Solo se permite reingreso si el empleado anterior fue terminado/finiquitado.");
        }

        // Reingreso válido → retornar ID anterior para vincular historial
        return existing.Id;
    }

    /// <summary>
    /// Valida formato y unicidad de cédula para ACTUALIZACIÓN, excluyendo el propio empleado.
    /// Usado EXCLUSIVAMENTE por UpdateEmployeeCommandHandler cuando cambia la cédula.
    /// </summary>
    public async Task ValidateIdentificationForUpdateAsync(
        Guid employeeId, string identification, CancellationToken ct)
    {
        if (!CedulaValidator.EsValida(identification))
            throw new BusinessRuleViolationException(
                $"La cédula '{identification}' no es válida según el algoritmo de verificación nicaragüense.");

        var existing = await _employeeRepo.GetByIdentificationAsync(identification, ct);

        // Si existe otro empleado (no el que se edita) → violación de unicidad
        if (existing != null && existing.Id != employeeId)
        {
            throw new BusinessRuleViolationException(
                $"Ya existe otro empleado con la cédula {identification}. " +
                $"Empleado conflictivo: {existing.Code} - {existing.FirstName} {existing.LastName}. " +
                "No se puede asignar la misma cédula a dos empleados diferentes.");
        }
    }

    /// <summary>
    /// Valida campos obligatorios básicos para actualización.
    /// Separa validaciones de entrada de reglas complejas de dominio.
    /// </summary>
    public void ValidateRequiredFieldsForUpdate(
        string? identification, int? departmentId, int? municipalityId)
    {
        if (string.IsNullOrWhiteSpace(identification))
            throw new BusinessRuleViolationException("La identificación/cédula es obligatoria.");

        if (!departmentId.HasValue || departmentId.Value == 0)
            throw new BusinessRuleViolationException("El departamento es obligatorio.");

        if (!municipalityId.HasValue || municipalityId.Value == 0)
            throw new BusinessRuleViolationException("El municipio es obligatorio.");
    }

    /// <summary>
    /// Valida puesto/nivel activo y salario ≥ mínimo legal.
    /// Retorna JobGrade validado para extraer OccupationalRiskId después.
    /// Usado por CreateEmployeeCommandHandler.
    /// </summary>
    public async Task<JobGrade> ValidateJobGradeAndSalaryAsync(
        Guid jobGradeId, decimal baseSalary, CancellationToken ct)
    {
        var jobGrade = await _jobGradeRepo.GetByIdWithPositionAsync(jobGradeId, ct)
            ?? throw new BusinessRuleViolationException("El puesto/nivel seleccionado no existe o está inactivo");

        if (!jobGrade.IsActive)
            throw new BusinessRuleViolationException("El puesto/nivel seleccionado no existe o está inactivo");

        if (jobGrade.JobPosition.MinimumWageId.HasValue)
        {
            var minWage = await _minimumWageRepo.GetByIdAsync(jobGrade.JobPosition.MinimumWageId.Value, ct);
            if (minWage != null && baseSalary < minWage.MonthlyAmountNIO)
                throw new BusinessRuleViolationException(
                    $"El salario debe ser mayor o igual al mínimo legal: C$ {minWage.MonthlyAmountNIO:N2}");
        }

        return jobGrade;
    }

    /// <summary>
    /// Valida que departamento y municipio existan, estén activos y sean compatibles.
    /// Usado por AMBOS handlers (Create y Update).
    /// </summary>
    public async Task<(Department Dept, Municipality Mun)> ValidateLocationAsync(
        int departmentId, int municipalityId, CancellationToken ct)
    {
        var dept = await _departmentRepo.GetByIdActiveAsync(departmentId, ct)
            ?? throw new BusinessRuleViolationException("El departamento seleccionado no existe o está inactivo.");

        var mun = await _municipalityRepo.GetByIdActiveInDepartmentAsync(municipalityId, departmentId, ct)
            ?? throw new BusinessRuleViolationException(
                "El municipio seleccionado no existe, está inactivo o no pertenece al departamento.");

        return (dept, mun);
    }

    /// <summary>
    /// Calcula período de prueba basado en tipo de contrato.
    /// Usado EXCLUSIVAMENTE por CreateEmployeeCommandHandler.
    /// </summary>
    public async Task<(DateTime? Start, DateTime? End)> CalculateProbationPeriodAsync(
        int contractTypeId, DateTime hireDate, CancellationToken ct)
    {
        var contractType = await _contractTypeRepo.GetByIdAsync(contractTypeId, ct);

        if (contractType == null || !contractType.HasProbationPeriod)
            return (null, null);

        return (hireDate, hireDate.AddDays(contractType.ProbationDays));
    }
}