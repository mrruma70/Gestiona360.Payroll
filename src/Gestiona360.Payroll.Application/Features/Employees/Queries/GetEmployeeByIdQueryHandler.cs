using Gestiona360.Payroll.Application.Contracts.DTOs.Employees;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.Employees.Queries;

public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeDetailDto>
{
    private readonly ApplicationDbContext _context;

    public GetEmployeeByIdQueryHandler(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<EmployeeDetailDto> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees
            .Include(e => e.Company)
            .Include(e => e.Branch)
            .Include(e => e.CostCenter)
            .Include(e => e.ContractType)
            .Include(e => e.JobGrade)
                .ThenInclude(jg => jg!.JobPosition)
            .Include(e => e.Bank)
            .Include(e => e.HealthProvider)
            .Include(e => e.OccupationalRisk)
            // ✅ NUEVO: Incluir asignaciones de turno para mostrar el turno actual
            .Include(e => e.ShiftAssignments)
                .ThenInclude(sa => sa.Shift)
            .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken);

        if (employee == null)
            throw new KeyNotFoundException($"Empleado con ID {request.EmployeeId} no encontrado.");

        // ✅ Obtener el turno actual (el más reciente sin fecha fin o con fecha fin futura)
        var currentShiftAssignment = employee.ShiftAssignments
            .Where(sa => sa.EndDate == null || sa.EndDate > DateTime.UtcNow)
            .OrderByDescending(sa => sa.StartDate)
            .FirstOrDefault();

        return new EmployeeDetailDto
        {
            // === DATOS BÁSICOS ===
            Id = employee.Id,
            Code = employee.Code,
            Identification = employee.Identification,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            Phone = employee.Phone,
            HireDate = employee.HireDate,
            TerminationDate = employee.TerminationDate,
            IsActive = employee.IsActive,

            // === EMPRESA ===
            CompanyId = employee.CompanyId,
            CompanyName = employee.Company?.LegalName ?? "N/A",

            // === SUCURSAL ===
            BranchId = employee.BranchId,
            BranchName = employee.Branch?.Name ?? "N/A",
            BranchCode = employee.Branch?.Code ?? "N/A",

            // === CONTRATO ===
            ContractTypeId = (int)employee.ContractTypeId,
            ContractTypeName = employee.ContractType?.Name ?? "N/A",

            // === PUESTO/NIVEL ===
            JobGradeId = employee.JobGradeId,
            JobPositionName = employee.JobGrade?.JobPosition?.Name ?? "N/A",
            JobGradeName = employee.JobGrade?.Name ?? "N/A",

            // === SALARIO ===
            BaseSalary = employee.BaseSalary,

            // === PROVEEDOR SALUD ===
            HealthProviderId = employee.HealthProviderId,
            HealthProviderName = employee.HealthProvider?.Name ?? "No asignado",

            // === BANCO ===
            BankId = employee.BankId,
            BankName = employee.Bank?.Name ?? "No asignado",
            BankAccountNumber = employee.BankAccountNumber ?? "N/A",
            BankAccountType = employee.BankAccountType ?? "N/A",

            // === RIESGO OCUPACIONAL ===
            OccupationalRiskId = employee.OccupationalRiskId,
            OccupationalRiskName = employee.OccupationalRisk?.Name ?? "N/A",

            // === DOCUMENTOS ===
            PhotoUrl = employee.PhotoUrl,
            IdFrontUrl = employee.IdFrontUrl,
            IdBackUrl = employee.IdBackUrl,

            // === DATOS FISCALES ===
            NORUC = employee.NORUC ?? string.Empty,
            NOINSS = employee.NOINSS ?? string.Empty,

            // === ESTADO Y SUSPENSIÓN ===
            EmploymentStatus = employee.EmploymentStatus,
            SuspensionStartDate = employee.SuspensionStartDate,
            SuspensionEndDate = employee.SuspensionEndDate,
            SuspensionJustification = employee.SuspensionJustification,
            MitrabAuthorizationNumber = employee.MitrabAuthorizationNumber,

            // === REINGRESO ===
            PreviousEmployeeId = employee.PreviousEmployeeId,

            // === PERÍODO DE PRUEBA ===
            ProbationStartDate = employee.ProbationStartDate,
            ProbationEndDate = employee.ProbationEndDate,
            // ✅ Propiedad calculada
            IsOnProbation = employee.ProbationStartDate.HasValue &&
                            DateTime.UtcNow >= employee.ProbationStartDate.Value &&
                            (!employee.ProbationEndDate.HasValue || DateTime.UtcNow <= employee.ProbationEndDate.Value),

            // === CONDICIONES ESPECIALES ===
            IsTrustEmployee = employee.IsTrustEmployee,
            BenefitsInKindValue = employee.BenefitsInKindValue,
            BenefitsInKindDescription = employee.BenefitsInKindDescription,

            // === TRABAJADOR EXTRANJERO ===
            Nationality = employee.Nationality,
            WorkPermitNumber = employee.WorkPermitNumber,
            WorkPermitExpirationDate = employee.WorkPermitExpirationDate,

            // === NOTAS ===
            Notes = employee.Notes,

            // === TURNO ACTUAL (NUEVO) ===
            CurrentShiftName = currentShiftAssignment?.Shift?.Name,
            CurrentShiftSchedule = currentShiftAssignment != null && currentShiftAssignment.Shift != null
                ? $"{currentShiftAssignment.Shift.StartTime:hh\\:mm} - {currentShiftAssignment.Shift.EndTime:hh\\:mm}"
                : null,
            ShiftAssignmentStartDate = currentShiftAssignment?.StartDate,

            CostCenterId = employee.CostCenterId,
            CostCenterName = employee.CostCenter?.Name ?? "No asignado",
            CostCenterCode = employee.CostCenter?.Code ?? ""
        };
    }
}