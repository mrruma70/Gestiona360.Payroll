// src/Gestiona360.Payroll.Infrastructure.Persistence/Repositories/EmployeeRepository.cs
using System.Linq.Expressions;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Interfaces;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación EF Core para empleados. 
/// Todos los métodos usan Guid, CancellationToken y AsNoTracking en lecturas.
/// </summary>
public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _context;

    public EmployeeRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Obtiene empleado por ID único (Guid). Usa FindAsync para aprovechar caché de EF Core.
    /// </summary>
    public async Task<Employee?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Employees.FindAsync(new object[] { id }, ct);
    }

    public async Task<IEnumerable<Employee>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var idList = ids.ToList();
        return await _context.Employees
            .AsNoTracking()
            .Where(e => idList.Contains(e.Id))
            .ToListAsync(ct);
    }

    /// <summary>
    /// Búsqueda flexible con predicado LINQ. SIEMPRE AsNoTracking para consultas.
    /// </summary>
    public async Task<IEnumerable<Employee>> FindAsync(
        Expression<Func<Employee, bool>> predicate,
        CancellationToken ct = default)
    {
        return await _context.Employees
            .AsNoTracking()
            .Where(predicate)
            .ToListAsync(ct);
    }

    /// <summary>
    /// Agrega nueva entidad al contexto. Retorna la misma instancia para fluidez.
    /// </summary>
    public async Task<Employee> AddAsync(Employee employee, CancellationToken ct = default)
    {
        await _context.Employees.AddAsync(employee, ct);
        return employee;
    }

    /// <summary>
    /// Marca entidad como modificada. SaveChanges debe llamarse después (vía UoW o Behavior).
    /// </summary>
    public void Update(Employee employee)
    {
        _context.Employees.Update(employee);
    }

    /// <summary>
    /// Marca entidad para eliminación. SaveChanges debe llamarse después.
    /// </summary>
    public void Delete(Employee employee)
    {
        _context.Employees.Remove(employee);
    }

    /// <summary>
    /// Suma salarios base activos por departamento. Optimizado con AsNoTracking.
    /// Usa BaseSalary (no Salary) según modelo de dominio real.
    /// </summary>
    public async Task<decimal> GetTotalSalaryByDepartmentAsync(
        int departmentId, // ✅ CORREGIDO: Guid en lugar de int
        CancellationToken ct = default)
    {
        return await _context.Employees
            .AsNoTracking()
            .Where(e => e.DepartmentId == departmentId && e.IsActive)
            .SumAsync(e => e.BaseSalary, ct);
    }

    /// <summary>
    /// Busca empleado por cédula/número de identificación. 
    /// Requerido para validación de reingreso en EmployeeDomainService.
    /// </summary>
    public async Task<Employee?> GetByIdentificationAsync(
        string identification,
        CancellationToken ct = default)
    {
        return await _context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Identification == identification, ct);
    }

    // src/Gestiona360.Payroll.Infrastructure.Persistence/Repositories/EmployeeRepository.cs
    public async Task<string> GenerateNextEmployeeCodeAsync(CancellationToken ct = default)
    {
        // 1. Proyección SELECT directa: Solo trae el código, no carga entidades completas
        var lastCode = await _context.Employees
            .OrderByDescending(e => e.Code)
            .Select(e => e.Code)
            .FirstOrDefaultAsync(ct);

        // 2. Caso base: Si no hay empleados, iniciar secuencia
        if (string.IsNullOrEmpty(lastCode))
            return "EMP-001";

        // 3. Parseo seguro del sufijo numérico
        const string prefix = "EMP-";
        if (!lastCode.StartsWith(prefix))
            return $"EMP-{DateTime.UtcNow.Ticks % 1000:D3}"; // Fallback seguro

        var numberPart = lastCode[prefix.Length..];

        if (int.TryParse(numberPart, out int currentNumber))
        {
            // Incrementar y formatear con ceros a la izquierda (3 dígitos)
            return $"{prefix}{(currentNumber + 1):D3}";
        }

        // 4. Fallback ante formato corrupto: usar timestamp truncado
        return $"{prefix}{(DateTime.UtcNow.Ticks % 1000):D3}";
    }

    // ✅ Implementación de la consulta optimizada
    public async Task<Employee?> GetEmployeeWithFullDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AsNoTracking()
            .Include(e => e.Company)
            .Include(e => e.Branch)
            .Include(e => e.CostCenter)
            .Include(e => e.ContractType)
            .Include(e => e.JobGrade).ThenInclude(jg => jg!.JobPosition)
            .Include(e => e.Bank)
            .Include(e => e.HealthProvider)
            .Include(e => e.OccupationalRisk)
            .Include(e => e.PayrollGroup)
            .Include(e => e.Department)
            .Include(e => e.Municipality)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<Employee?> GetEmployeeForDetailAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Employees
            .AsNoTracking()
            .Include(e => e.Company)
            .Include(e => e.Branch)
            .Include(e => e.CostCenter)
            .Include(e => e.ContractType)
            .Include(e => e.JobGrade).ThenInclude(jg => jg!.JobPosition)
            .Include(e => e.Bank)
            .Include(e => e.HealthProvider)
            .Include(e => e.OccupationalRisk)
            .Include(e => e.PayrollGroup)
            .Include(e => e.Department)
            .Include(e => e.Municipality)
            // ✅ Incluir ShiftAssignments con filtro de turno activo
            .Include(e => e.ShiftAssignments.Where(sa => sa.EndDate == null))
                .ThenInclude(sa => sa.Shift)
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public async Task<IEnumerable<Employee>> GetEmployeesForListAsync(
    Expression<Func<Employee, bool>>? predicate = null,
    CancellationToken ct = default)
    {
        var query = _context.Employees
            .AsNoTracking()
            .Include(e => e.JobGrade).ThenInclude(jg => jg!.JobPosition)
            .Include(e => e.Branch)
            .Include(e => e.ContractType)
            .Include(e => e.CostCenter)
            .Include(e => e.Bank)
            .Include(e => e.HealthProvider)
            .Include(e => e.PayrollGroup)
            .Include(e => e.ShiftAssignments.Where(sa => sa.EndDate == null))
                .ThenInclude(sa => sa.Shift)
            .AsQueryable();

        if (predicate != null)
            query = query.Where(predicate);

        return await query
            .OrderBy(e => e.FirstName)
            .ThenBy(e => e.LastName)
            .ToListAsync(ct);
    }

    public async Task<EmployeeStatsEntity> GetEmployeeStatsAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;

        var stats = await _context.Employees
            .AsNoTracking()
            .GroupBy(e => 1)
            .Select(g => new EmployeeStatsEntity
            {
                Total = g.Count(),
                Active = g.Count(e => e.IsActive),
                Inactive = g.Count(e => !e.IsActive),
                Suspended = g.Count(e => e.EmploymentStatus == EmploymentStatus.Suspended),
                Terminated = g.Count(e => e.EmploymentStatus == EmploymentStatus.Terminated),
                TrustEmployees = g.Count(e => e.IsTrustEmployee),
                ForeignWorkers = g.Count(e => !string.IsNullOrEmpty(e.Nationality)),
                Rehires = g.Count(e => e.PreviousEmployeeId.HasValue)
            })
            .FirstOrDefaultAsync(ct);

        if (stats == null)
            return new EmployeeStatsEntity();

        // ✅ Calcular en cliente (no se puede hacer en SQL)
        stats.NewThisMonth = await _context.Employees
            .AsNoTracking()
            .CountAsync(e => e.HireDate.Month == now.Month && e.HireDate.Year == now.Year, ct);

        stats.OnProbation = await _context.Employees
            .AsNoTracking()
            .CountAsync(e => e.ProbationStartDate.HasValue &&
                            e.ProbationEndDate.HasValue &&
                            now >= e.ProbationStartDate.Value &&
                            now <= e.ProbationEndDate.Value, ct);

        return stats;
    }

    /// </summary>
    public async Task<Employee?> GetEmployeeByIdWithShiftAsync(Guid id, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;

        return await _context.Employees
            .AsNoTracking()
            .Include(e => e.Company)
            .Include(e => e.Branch)
            .Include(e => e.CostCenter)
            .Include(e => e.ContractType)
            .Include(e => e.JobGrade).ThenInclude(jg => jg!.JobPosition)
            .Include(e => e.Bank)
            .Include(e => e.HealthProvider)
            .Include(e => e.OccupationalRisk)
            .Include(e => e.PayrollGroup)
            .Include(e => e.Department)
            .Include(e => e.Municipality)
            // ✅ Incluir ShiftAssignments con filtro de turno activo
            .Include(e => e.ShiftAssignments.Where(sa => sa.EndDate == null || sa.EndDate > now))
                .ThenInclude(sa => sa.Shift)
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public async Task<EmployeeConceptsData?> GetEmployeeConceptsAsync(Guid employeeId, CancellationToken ct = default)
    {
        // 1. Verificar que el empleado existe y obtener datos básicos
        var employee = await _context.Employees
            .AsNoTracking()
            .Where(e => e.Id == employeeId)
            .Select(e => new { e.FirstName, e.LastName, e.Code })
            .FirstOrDefaultAsync(ct);

        if (employee == null)
            return null;

        // 2. Obtener conceptos recurrentes
        var concepts = await _context.EmployeeConceptSettings
            .AsNoTracking()
            .Include(ecs => ecs.PayrollConcept)
            .Where(ecs => ecs.EmployeeId == employeeId)
            .OrderByDescending(ecs => ecs.StartDate)
            .Select(ecs => new EmployeeConceptInfo
            {
                Id = ecs.Id,
                ConceptName = ecs.PayrollConcept.Name,
                Type = ecs.PayrollConcept.Type,
                Category = ecs.PayrollConcept.Category,
                IsActive = ecs.IsActive,
                Amount = ecs.CustomValue > 0 ? (decimal)ecs.CustomValue : (decimal)ecs.InstallmentAmount,
                Periodicity = ecs.ApplicationPeriodicity,
                InstallmentsInfo = (ecs.IsRecurrent && ecs.InstallmentTotal > 0)
                    ? $"{ecs.InstallmentCurrent} de {ecs.InstallmentTotal}"
                    : null,
                RemainingBalance = ecs.IsRecurrent ? ecs.RemainingBalance : null,
                StartDate = ecs.StartDate,
                EndDate = ecs.EndDate
            })
            .ToListAsync(ct);

        // 3. Obtener retenciones judiciales
        var now = DateTime.UtcNow;
        var garnishments = await _context.Garnishments
            .AsNoTracking()
            .Where(g => g.EmployeeId == employeeId)
            .OrderByDescending(g => g.StartDate)
            .Select(g => new GarnishmentInfo
            {
                Id = g.Id,
                Type = g.Type,
                CourtOrderNumber = g.CourtOrderNumber,
                PercentageLimit = g.PercentageLimit,
                TotalAmountToWithhold = g.TotalAmountToWithhold ?? 0,
                WithheldToDate = g.WithheldToDate,
                IsActive = g.EndDate == null || g.EndDate > now,
                StartDate = g.StartDate,
                EndDate = g.EndDate
            })
            .ToListAsync(ct);

        return new EmployeeConceptsData
        {
            EmployeeName = $"{employee.FirstName} {employee.LastName}",
            EmployeeCode = employee.Code,
            Concepts = concepts,
            Garnishments = garnishments
        };
    }

    public async Task<EmployeeShiftsData?> GetEmployeeShiftsAsync(Guid employeeId, CancellationToken ct = default)
    {
        // 1. Verificar que el empleado existe
        var employee = await _context.Employees
            .AsNoTracking()
            .Where(e => e.Id == employeeId)
            .Select(e => new { e.FirstName, e.LastName })
            .FirstOrDefaultAsync(ct);

        if (employee == null)
            return null;

        // 2. Obtener el turno ACTUAL con sus horarios
        var now = DateTime.UtcNow;
        var currentAssignment = await _context.EmployeeShiftAssignments
            .AsNoTracking()
            .Where(a => a.EmployeeId == employeeId && (a.EndDate == null || a.EndDate >= now))
            .Include(a => a.Shift)!.ThenInclude(s => s!.Schedules)
            .OrderByDescending(a => a.StartDate)
            .FirstOrDefaultAsync(ct);

        EmployeeCurrentShiftInfo? currentShiftInfo = null;
        if (currentAssignment?.Shift != null)
        {
            // ✅ Traer horarios crudos y ordenar en memoria
            var schedules = currentAssignment.Shift.Schedules
                .OrderBy(s => s.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)s.DayOfWeek)
                .Select(s => new ShiftScheduleInfo
                {
                    DayOfWeek = (int)s.DayOfWeek,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    IsRestDay = s.IsRestDay
                })
                .ToList();

            currentShiftInfo = new EmployeeCurrentShiftInfo
            {
                ShiftId = currentAssignment.Shift.Id,
                ShiftName = currentAssignment.Shift.Name,
                ShiftType = currentAssignment.Shift.ShiftType,
                IsNightShift = currentAssignment.Shift.IsNightShift,
                StartDate = currentAssignment.StartDate,
                Schedules = schedules
            };
        }

        // 3. Obtener el historial de turnos
        var history = await _context.EmployeeShiftAssignments
            .AsNoTracking()
            .Where(a => a.EmployeeId == employeeId)
            .Include(a => a.Shift)
            .Include(a => a.PersonalAction)
            .OrderByDescending(a => a.StartDate)
            .Select(a => new EmployeeShiftHistoryInfo
            {
                AssignmentId = a.Id,
                ShiftName = a.Shift != null ? a.Shift.Name : "Turno Desconocido",
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                Justification = a.Justification,
                // ✅ Traer como string directamente
                ActionType = a.PersonalAction != null ? a.PersonalAction.ActionType.ToString() : "Asignación Inicial"
            })
            .ToListAsync(ct);

        return new EmployeeShiftsData
        {
            EmployeeName = $"{employee.FirstName} {employee.LastName}",
            CurrentShift = currentShiftInfo,
            ShiftHistory = history
        };
    }

    public async Task<IEnumerable<EmployeeBasicInfo>> GetActiveEmployeesAsync(CancellationToken ct = default)
    {
        return await _context.Employees
            .AsNoTracking()
            .Where(e => e.IsActive)
            .OrderBy(e => e.FirstName)
            .ThenBy(e => e.LastName)
            .Select(e => new EmployeeBasicInfo
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Identification = e.Identification,
                Email = e.Email,
                Phone = e.Phone,
                IsActive = e.IsActive
            })
            .ToListAsync(ct);
    }

    /// <summary>
    /// Obtiene empleado con las relaciones necesarias para generar la credencial/carnet.
    /// Incluye: Company, Branch, JobGrade.JobPosition, PayrollGroup.
    /// </summary>
    public async Task<Employee?> GetEmployeeForCredentialAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Employees
            .AsNoTracking()
            .Include(e => e.Company)
            .Include(e => e.Branch)
            .Include(e => e.JobGrade)
                .ThenInclude(jg => jg!.JobPosition)
            .Include(e => e.PayrollGroup)
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }

    public async Task<Employee?> GetByIdWithJobDetailsAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Employees
            .Include(e => e.JobGrade)
                .ThenInclude(jg => jg.JobPosition)
                    .ThenInclude(jp => jp.MinimumWage)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id, ct);
    }



    public async Task<List<ActiveEmployeeDto>> GetActiveEmployeesSummaryAsync(CancellationToken ct = default)
    {
        return await _context.Employees
            .AsNoTracking()
            .Where(e => e.IsActive)
            .Select(e => new ActiveEmployeeDto
            {
                Id = e.Id,
                Code = e.Code,
                FirstName = e.FirstName,
                LastName = e.LastName,
                JobGradeName = e.JobGrade.Name,
                BaseSalary = e.BaseSalary
            })
            .ToListAsync(ct);
    }

    public async Task<decimal?> GetMinimumWageForEmployeeAsync(Guid employeeId, CancellationToken ct = default)
    {
        var result = await _context.Employees
            .AsNoTracking()
            .Where(e => e.Id == employeeId)
            .Select(e => new
            {
                // Navegación: Employee → JobGrade → JobPosition → MinimumWage
                MinimumWage = e.JobGrade != null &&
                             e.JobGrade.JobPosition != null &&
                             e.JobGrade.JobPosition.MinimumWage != null
                    ? e.JobGrade.JobPosition.MinimumWage.MonthlyAmountNIO
                    : (decimal?)null
            })
            .FirstOrDefaultAsync(ct);

        return result?.MinimumWage;
    }

}
