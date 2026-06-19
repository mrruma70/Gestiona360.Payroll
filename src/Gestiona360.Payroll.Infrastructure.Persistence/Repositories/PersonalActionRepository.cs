// src/Gestiona360.Payroll.Infrastructure.Persistence/Repositories/PersonalActionRepository.cs

using Gestiona360.Payroll.Application.Contracts.DTOs.PersonalActions;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Interfaces;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using Gestiona360.Payroll.Domain.Shared.Frontend.Enums;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Infrastructure.Persistence.Repositories;

public class PersonalActionRepository : IPersonalActionRepository
{
    private readonly ApplicationDbContext _context;

    public PersonalActionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtiene acciones personales con todos los detalles relacionados mediante JOINs.
    /// </summary>
    public async Task<IEnumerable<PersonalActionWithDetails>> GetPersonalActionsWithDetailsAsync(
        Guid employeeId,
        CancellationToken ct = default)
    {
        var query = from pa in _context.PersonalActions
                    join u in _context.Users
                        on pa.ApprovedBy equals u.Id into users
                    from u in users.DefaultIfEmpty()

                    join oldJobGrade in _context.JobGrades
                        on pa.OldJobGradeId equals oldJobGrade.Id into oldJobGrades
                    from oldJobGrade in oldJobGrades.DefaultIfEmpty()

                    join newJobGrade in _context.JobGrades
                        on pa.NewJobGradeId equals newJobGrade.Id into newJobGrades
                    from newJobGrade in newJobGrades.DefaultIfEmpty()

                    join oldContractType in _context.ContractTypes
                        on pa.OldContractTypeId equals oldContractType.Id into oldContractTypes
                    from oldContractType in oldContractTypes.DefaultIfEmpty()

                    join newContractType in _context.ContractTypes
                        on pa.NewContractTypeId equals newContractType.Id into newContractTypes
                    from newContractType in newContractTypes.DefaultIfEmpty()

                    join oldShift in _context.Shifts
                        on pa.OldShiftId equals oldShift.Id into oldShifts
                    from oldShift in oldShifts.DefaultIfEmpty()

                    join newShift in _context.Shifts
                        on pa.NewShiftId equals newShift.Id into newShifts
                    from newShift in newShifts.DefaultIfEmpty()

                    join oldCostCenter in _context.CostCenters
                        on pa.OldCostCenterId equals oldCostCenter.Id into oldCostCenters
                    from oldCostCenter in oldCostCenters.DefaultIfEmpty()

                    join newCostCenter in _context.CostCenters
                        on pa.NewCostCenterId equals newCostCenter.Id into newCostCenters
                    from newCostCenter in newCostCenters.DefaultIfEmpty()

                    where pa.EmployeeId == employeeId
                    orderby pa.EffectiveDate descending
                    select new PersonalActionWithDetails
                    {
                        Id = pa.Id,
                        EmployeeId = pa.EmployeeId,
                        EffectiveDate = pa.EffectiveDate,
                        ActionType = pa.ActionType,
                        Status = pa.Status,
                        Justification = pa.Justification,
                        CausalDescription = pa.CausalDescription,
                        ExecutedDate = pa.ExecutedDate,
                        ApprovedBy = pa.ApprovedBy,
                        ApprovedByName = u != null ? (u.UserName ?? u.Email ?? "Sistema") : "Sistema",

                        OldBaseSalary = pa.OldBaseSalary,
                        NewBaseSalary = pa.NewBaseSalary,

                        OldJobGradeId = pa.OldJobGradeId,
                        OldJobGradeName = oldJobGrade != null ? oldJobGrade.Name : null,
                        NewJobGradeId = pa.NewJobGradeId,
                        NewJobGradeName = newJobGrade != null ? newJobGrade.Name : null,

                        OldContractTypeId = pa.OldContractTypeId,
                        OldContractTypeName = oldContractType != null ? oldContractType.Name : null,
                        NewContractTypeId = pa.NewContractTypeId,
                        NewContractTypeName = newContractType != null ? newContractType.Name : null,

                        OldShiftId = pa.OldShiftId,
                        OldShiftName = oldShift != null ? oldShift.Name : null,
                        NewShiftId = pa.NewShiftId,
                        NewShiftName = newShift != null ? newShift.Name : null,

                        OldCostCenterId = pa.OldCostCenterId,
                        OldCostCenterName = oldCostCenter != null ? oldCostCenter.Name : null,
                        NewCostCenterId = pa.NewCostCenterId,
                        NewCostCenterName = newCostCenter != null ? newCostCenter.Name : null
                    };

        return await query.ToListAsync(ct);
    }

    public async Task<PersonalAction?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.PersonalActions.FindAsync(new object[] { id }, ct);
    }
    public async Task<bool> ExistsByBatchReferenceAsync(string batchReference, CancellationToken ct = default)
    {
        return await _context.PersonalActions
            .AsNoTracking()
            .AnyAsync(a => a.BatchReference == batchReference, ct);
    }
    public async Task<IEnumerable<Employee>> GetEmployeesByIdsAsync(
        IEnumerable<Guid> employeeIds,
        CancellationToken ct = default)
    {
        return await _context.Employees
            .AsNoTracking()
            .Where(e => employeeIds.Contains(e.Id))
            .ToListAsync(ct);
    }

    public async Task<PayrollPeriod?> GetPeriodByDateAndPayrollGroupAsync(
        Guid payrollGroupId,
        DateTime effectiveDate,
        CancellationToken ct = default)
    {
        return await _context.PayrollPeriods
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.PayrollGroupId == payrollGroupId &&
                                      effectiveDate >= p.StartDate &&
                                      effectiveDate <= p.EndDate, ct);
    }

    public async Task AddRangeAsync(IEnumerable<PersonalAction> actions, CancellationToken ct = default)
    {
        await _context.PersonalActions.AddRangeAsync(actions, ct);
    }

    public async Task AddAsync(PersonalAction action, CancellationToken ct = default)
    {
        await _context.PersonalActions.AddAsync(action, ct);
    }

    public void Delete(PersonalAction action)
    {
        _context.PersonalActions.Remove(action);
    }


    public void DeleteRange(IEnumerable<PersonalAction> actions)
    {
        _context.PersonalActions.RemoveRange(actions);
    }

    public async Task<IEnumerable<PersonalAction>> GetPendingByBatchReferenceAsync(
    string batchReference,
    CancellationToken ct = default)
    {
        return await _context.PersonalActions
            .Where(a => a.BatchReference == batchReference && a.Status == ActionStatus.Pending)
            .ToListAsync(ct);
    }

    public void Update(PersonalAction action)
    {
        _context.PersonalActions.Update(action);
    }

    public async Task<PersonalAction?> GetByIdWithEmployeeAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.PersonalActions
            .Include(a => a.Employee)
            .FirstOrDefaultAsync(a => a.Id == id, ct);
    }

    // ✅ AGREGAR: Consulta de detalle con todos los JOINs
    public async Task<PersonalActionDetailDto?> GetDetailByIdAsync(Guid id, CancellationToken ct = default)
    {
        var result = await (
            from pa in _context.PersonalActions.AsNoTracking()

            // JOIN con Employee
            join emp in _context.Employees.AsNoTracking()
                on pa.EmployeeId equals emp.Id into empGroup
            from emp in empGroup.DefaultIfEmpty()

                // JOIN con PayrollGroup
            join pg in _context.PayrollGroups.AsNoTracking()
                on pa.PayrollGroupId equals pg.Id into pgGroup
            from pg in pgGroup.DefaultIfEmpty()

                // JOIN con PayrollPeriod
            join pp in _context.PayrollPeriods.AsNoTracking()
                on pa.TargetPayrollPeriodId equals pp.Id into ppGroup
            from pp in ppGroup.DefaultIfEmpty()

                // JOIN con JobGrade (Antiguo)
            join oldJg in _context.JobGrades.AsNoTracking()
                on pa.OldJobGradeId equals oldJg.Id into oldJgGroup
            from oldJg in oldJgGroup.DefaultIfEmpty()

                // JOIN con JobGrade (Nuevo)
            join newJg in _context.JobGrades.AsNoTracking()
                on pa.NewJobGradeId equals newJg.Id into newJgGroup
            from newJg in newJgGroup.DefaultIfEmpty()

                // JOIN con ContractType (Antiguo)
            join oldCt in _context.ContractTypes.AsNoTracking()
                on pa.OldContractTypeId equals oldCt.Id into oldCtGroup
            from oldCt in oldCtGroup.DefaultIfEmpty()

                // JOIN con ContractType (Nuevo)
            join newCt in _context.ContractTypes.AsNoTracking()
                on pa.NewContractTypeId equals newCt.Id into newCtGroup
            from newCt in newCtGroup.DefaultIfEmpty()

                // JOIN con Shift (Antiguo)
            join oldSh in _context.Shifts.AsNoTracking()
                on pa.OldShiftId equals oldSh.Id into oldShGroup
            from oldSh in oldShGroup.DefaultIfEmpty()

                // JOIN con Shift (Nuevo)
            join newSh in _context.Shifts.AsNoTracking()
                on pa.NewShiftId equals newSh.Id into newShGroup
            from newSh in newShGroup.DefaultIfEmpty()

                // JOIN con CostCenter (Antiguo)
            join oldCc in _context.CostCenters.AsNoTracking()
                on pa.OldCostCenterId equals oldCc.Id into oldCcGroup
            from oldCc in oldCcGroup.DefaultIfEmpty()

                // JOIN con CostCenter (Nuevo)
            join newCc in _context.CostCenters.AsNoTracking()
                on pa.NewCostCenterId equals newCc.Id into newCcGroup
            from newCc in newCcGroup.DefaultIfEmpty()

                // JOIN con HealthProvider (Antiguo)
            join oldHp in _context.HealthProviders.AsNoTracking()
                on pa.OldHealthProviderId equals oldHp.Id into oldHpGroup
            from oldHp in oldHpGroup.DefaultIfEmpty()

                // JOIN con HealthProvider (Nuevo)
            join newHp in _context.HealthProviders.AsNoTracking()
                on pa.NewHealthProviderId equals newHp.Id into newHpGroup
            from newHp in newHpGroup.DefaultIfEmpty()

                // JOIN con Bank (Antiguo)
            join oldBk in _context.Banks.AsNoTracking()
                on pa.OldBankId equals oldBk.Id into oldBkGroup
            from oldBk in oldBkGroup.DefaultIfEmpty()

                // JOIN con Bank (Nuevo)
            join newBk in _context.Banks.AsNoTracking()
                on pa.NewBankId equals newBk.Id into newBkGroup
            from newBk in newBkGroup.DefaultIfEmpty()

                // JOIN con User (Aprobador/Ejecutor)
            join user in _context.Users.AsNoTracking()
                on pa.ApprovedBy equals user.Id into userGroup
            from user in userGroup.DefaultIfEmpty()

                // JOIN con User (Creador)
            join creator in _context.Users.AsNoTracking()
                on pa.CreatedBy equals creator.Id into creatorGroup
            from creator in creatorGroup.DefaultIfEmpty()

            where pa.Id == id
            select new PersonalActionDetailDto
            {
                // Datos básicos
                Id = pa.Id,
                EffectiveDate = pa.EffectiveDate,
                BatchReference = pa.BatchReference,

                // Datos del empleado
                EmployeeCode = emp != null ? emp.Code : null,
                EmployeeName = emp != null ? (emp.FirstName + " " + emp.LastName) : null,
                EmployeeIdentification = emp != null ? emp.Identification : null,
                EmployeePosition = emp != null ? emp.JobGrade.Name : null,

                // Contexto de nómina
                PayrollGroupId = pa.PayrollGroupId,
                PayrollGroupName = pg != null ? pg.Name : "Desconocido",
                TargetPayrollPeriodId = pa.TargetPayrollPeriodId,
                PayrollPeriodName = pp != null ? pp.PeriodNumber.ToString() : "Desconocido",
                PayrollPeriodStatus = pp != null ? pp.Status.ToString() : "Desconocido",

                // Tipo y causal
                ActionType = pa.ActionType.ToString(),
                ActionTypeDescription = GetActionTypeDescription(pa.ActionType),
                CausalDescription = pa.CausalDescription,

                // Estado
                Status = pa.Status.ToString(),

                // Justificación y documentos
                Justification = pa.Justification,
                DocumentAttachmentsJson = pa.DocumentAttachments,

                // Campos de suspensión
                SuspensionType = pa.SuspensionType.ToString(),
                SuspensionStartDate = pa.SuspensionStartDate,
                SuspensionEndDate = pa.SuspensionEndDate,

                // Campos de terminación
                TerminationTypeName = pa.TerminationTypeName,
                IndemnityAmount = pa.IndemnityAmount,
                IsJustified = pa.IsJustified,
                RejectionReason = pa.RejectionReason,

                // Auditoría
                CreatedDate = pa.CreatedAt,
                CreatedByName = creator != null ? (creator.UserName ?? "Sistema") : "Sistema",
                ApprovedDate = pa.ExecutedDate,
                ApprovedByName = user != null ? (user.UserName ?? "Sistema") : "Sistema",
                ExecutedDate = pa.ExecutedDate,
                ExecutedByName = user != null ? (user.UserName ?? "Sistema") : "Sistema",

                // Valores fuertemente tipados - Salario
                OldBaseSalary = pa.OldBaseSalary,
                NewBaseSalary = pa.NewBaseSalary,

                // Valores fuertemente tipados - Puesto
                OldJobGradeId = pa.OldJobGradeId,
                OldJobGradeName = oldJg != null ? oldJg.Name : null,
                NewJobGradeId = pa.NewJobGradeId,
                NewJobGradeName = newJg != null ? newJg.Name : null,

                // Valores fuertemente tipados - Contrato
                OldContractTypeId = pa.OldContractTypeId,
                OldContractTypeName = oldCt != null ? oldCt.Name : null,
                NewContractTypeId = pa.NewContractTypeId,
                NewContractTypeName = newCt != null ? newCt.Name : null,

                // Valores fuertemente tipados - Turno
                OldShiftId = pa.OldShiftId,
                OldShiftName = oldSh != null ? oldSh.Name : null,
                NewShiftId = pa.NewShiftId,
                NewShiftName = newSh != null ? newSh.Name : null,

                // Valores fuertemente tipados - Centro de Costo
                OldCostCenterId = pa.OldCostCenterId,
                OldCostCenterName = oldCc != null ? oldCc.Name : null,
                NewCostCenterId = pa.NewCostCenterId,
                NewCostCenterName = newCc != null ? newCc.Name : null,

                // Valores fuertemente tipados - Proveedor de Salud
                OldHealthProviderId = pa.OldHealthProviderId,
                OldHealthProviderName = oldHp != null ? oldHp.Name : null,
                NewHealthProviderId = pa.NewHealthProviderId,
                NewHealthProviderName = newHp != null ? newHp.Name : null,

                // Valores fuertemente tipados - Banco y Cuenta
                OldBankId = pa.OldBankId,
                OldBankName = oldBk != null ? oldBk.Name : null,
                NewBankId = pa.NewBankId,
                NewBankName = newBk != null ? newBk.Name : null,
                OldBankAccountNumber = pa.OldBankAccountNumber,
                NewBankAccountNumber = pa.NewBankAccountNumber,

                // Control de procesamiento de nómina
                IsAppliedInPayroll = pa.IsAppliedInPayroll,
                AppliedInPayrollDate = pa.AppliedInPayrollDate,
                AppliedInPayrollRecordId = pa.AppliedInPayrollRecordId
            }
        ).FirstOrDefaultAsync(ct);

        return result;
    }

    // ✅ AGREGAR: Consulta de empleados del lote
    public async Task<List<EmployeeBriefDto>> GetBatchEmployeesAsync(string batchReference, CancellationToken ct = default)
    {
        return await (
            from pa in _context.PersonalActions.AsNoTracking()
            join emp in _context.Employees.AsNoTracking()
                on pa.EmployeeId equals emp.Id
            where pa.BatchReference == batchReference
            orderby emp.Code
            select new EmployeeBriefDto
            {
                Id = emp.Id,
                Code = emp.Code,
                FullName = emp.FirstName + " " + emp.LastName,
                Identification = emp.Identification,
                Position = emp.JobGrade.Name
            }
        ).ToListAsync(ct);
    }

    // ✅ AGREGAR: Método auxiliar para descripciones
    private static string GetActionTypeDescription(ActionType actionType) => actionType switch
    {
        ActionType.SalaryChange => "💰 Cambio de Salario",
        ActionType.PositionChange => "🔄 Cambio de Puesto",
        ActionType.ContractChange => "📄 Cambio de Contrato",
        ActionType.ShiftChange => "⏰ Cambio de Turno",
        ActionType.CostCenterChange => "🏢 Cambio de Centro de Costo",
        ActionType.HealthProviderChange => "🏥 Cambio de Proveedor de Salud",
        ActionType.BankAccountChange => "🏦 Cambio de Cuenta Bancaria",
        ActionType.Suspension => "⏸️ Suspensión",
        ActionType.MassSuspension => "🏭 Suspensión Colectiva",
        ActionType.Reinstatement => "▶️ Reincorporación",
        ActionType.Termination => "🚪 Terminación",
        ActionType.MassTermination => "🏚️ Terminación Masiva",
        _ => actionType.ToString()
    };

    // ✅ AGREGAR: Consulta paginada con filtros dinámicos
    public async Task<PersonalActionPagedResultDto> GetPagedAsync(
        PersonalActionFilterDto filter,
        CancellationToken ct = default)
    {
        // ═══════════════════════════════════════════════════════════════
        // QUERY BASE CON LEFT JOINS
        // ═══════════════════════════════════════════════════════════════

        var query = (
            from pa in _context.PersonalActions.AsNoTracking()

                // JOIN con Employee
            join emp in _context.Employees.AsNoTracking()
                on pa.EmployeeId equals emp.Id into empGroup
            from emp in empGroup.DefaultIfEmpty()

                // JOIN con JobGrade (Antiguo)
            join oldJg in _context.JobGrades.AsNoTracking()
                on pa.OldJobGradeId equals oldJg.Id into oldJgGroup
            from oldJg in oldJgGroup.DefaultIfEmpty()

                // JOIN con JobGrade (Nuevo)
            join newJg in _context.JobGrades.AsNoTracking()
                on pa.NewJobGradeId equals newJg.Id into newJgGroup
            from newJg in newJgGroup.DefaultIfEmpty()

                // JOIN con ContractType (Antiguo)
            join oldCt in _context.ContractTypes.AsNoTracking()
                on pa.OldContractTypeId equals oldCt.Id into oldCtGroup
            from oldCt in oldCtGroup.DefaultIfEmpty()

                // JOIN con ContractType (Nuevo)
            join newCt in _context.ContractTypes.AsNoTracking()
                on pa.NewContractTypeId equals newCt.Id into newCtGroup
            from newCt in newCtGroup.DefaultIfEmpty()

                // JOIN con Shift (Antiguo)
            join oldSh in _context.Shifts.AsNoTracking()
                on pa.OldShiftId equals oldSh.Id into oldShGroup
            from oldSh in oldShGroup.DefaultIfEmpty()

                // JOIN con Shift (Nuevo)
            join newSh in _context.Shifts.AsNoTracking()
                on pa.NewShiftId equals newSh.Id into newShGroup
            from newSh in newShGroup.DefaultIfEmpty()

                // JOIN con CostCenter (Antiguo)
            join oldCc in _context.CostCenters.AsNoTracking()
                on pa.OldCostCenterId equals oldCc.Id into oldCcGroup
            from oldCc in oldCcGroup.DefaultIfEmpty()

                // JOIN con CostCenter (Nuevo)
            join newCc in _context.CostCenters.AsNoTracking()
                on pa.NewCostCenterId equals newCc.Id into newCcGroup
            from newCc in newCcGroup.DefaultIfEmpty()

                // JOIN con User (Aprobador/Ejecutor)
            join user in _context.Users.AsNoTracking()
                on pa.ApprovedBy equals user.Id into userGroup
            from user in userGroup.DefaultIfEmpty()

                // ═══════════════════════════════════════════════════════════════
                // FILTRO OBLIGATORIO POR PERÍODO DE NÓMINA
                // ═══════════════════════════════════════════════════════════════
            where pa.TargetPayrollPeriodId == filter.PayrollPeriodId

            // ═══════════════════════════════════════════════════════════════
            // FILTROS OPCIONALES
            // ═══════════════════════════════════════════════════════════════

            // Filtro por grupo de nómina
            && (filter.PayrollGroupId == null || pa.PayrollGroupId == filter.PayrollGroupId)

            // Filtro por tipo de acción
            && (string.IsNullOrEmpty(filter.ActionType) || pa.ActionType.ToString() == filter.ActionType)

            // Filtro por estado
            && (string.IsNullOrEmpty(filter.Status) || pa.Status.ToString() == filter.Status)

            // Filtro por referencia de lote
            && (string.IsNullOrEmpty(filter.BatchReference) || pa.BatchReference == filter.BatchReference)

            // Filtro por empleado específico
            && (filter.EmployeeId == null || pa.EmployeeId == filter.EmployeeId)

            // Filtro de búsqueda libre
            && (string.IsNullOrEmpty(filter.SearchTerm) ||
                (emp != null && (
                    (emp.FirstName + " " + emp.LastName).Contains(filter.SearchTerm) ||
                    emp.Code.Contains(filter.SearchTerm) ||
                    emp.Identification.Contains(filter.SearchTerm)
                )) ||
                (pa.CausalDescription != null && pa.CausalDescription.Contains(filter.SearchTerm)) ||
                (pa.BatchReference != null && pa.BatchReference.Contains(filter.SearchTerm)))

            // ═══════════════════════════════════════════════════════════════
            // PROYECCIÓN AL DTO
            // ═══════════════════════════════════════════════════════════════
            select new PersonalActionListDto
            {
                // Identificadores
                Id = pa.Id,
                EffectiveDate = pa.EffectiveDate,
                BatchReference = pa.BatchReference,

                // Datos del empleado
                EmployeeCode = emp != null ? emp.Code : null,
                EmployeeName = emp != null ? (emp.FirstName + " " + emp.LastName) : null,
                EmployeeIdentification = emp != null ? emp.Identification : null,

                // Tipo y causal
                ActionType = pa.ActionType.ToString(),
                ActionTypeDescription = GetActionTypeDescription(pa.ActionType),
                CausalDescription = pa.CausalDescription,

                // Estado y auditoría
                Status = pa.Status.ToString(),
                ApprovedByName = user != null ? (user.UserName ?? "Sistema") : "Sistema",
                ExecutedDate = pa.ExecutedDate,

                // Valores fuertemente tipados - Salario
                OldBaseSalary = pa.OldBaseSalary,
                NewBaseSalary = pa.NewBaseSalary,

                // Valores fuertemente tipados - Puesto
                OldJobGradeId = pa.OldJobGradeId,
                OldJobGradeName = oldJg != null ? oldJg.Name : null,
                NewJobGradeId = pa.NewJobGradeId,
                NewJobGradeName = newJg != null ? newJg.Name : null,

                // Valores fuertemente tipados - Contrato
                OldContractTypeId = pa.OldContractTypeId,
                OldContractTypeName = oldCt != null ? oldCt.Name : null,
                NewContractTypeId = pa.NewContractTypeId,
                NewContractTypeName = newCt != null ? newCt.Name : null,

                // Valores fuertemente tipados - Turno
                OldShiftId = pa.OldShiftId,
                OldShiftName = oldSh != null ? oldSh.Name : null,
                NewShiftId = pa.NewShiftId,
                NewShiftName = newSh != null ? newSh.Name : null,

                // Valores fuertemente tipados - Centro de Costo
                OldCostCenterId = pa.OldCostCenterId,
                OldCostCenterName = oldCc != null ? oldCc.Name : null,
                NewCostCenterId = pa.NewCostCenterId,
                NewCostCenterName = newCc != null ? newCc.Name : null,

                // Control de procesamiento de nómina
                IsAppliedInPayroll = pa.IsAppliedInPayroll,
                AppliedInPayrollDate = pa.AppliedInPayrollDate
            }
        );

        // ═══════════════════════════════════════════════════════════════
        // ORDENAMIENTO DINÁMICO
        // ═══════════════════════════════════════════════════════════════

        query = filter.SortBy?.ToLower() switch
        {
            "effectivedate" => filter.SortDescending
                ? query.OrderByDescending(x => x.EffectiveDate)
                : query.OrderBy(x => x.EffectiveDate),
            "employeename" => filter.SortDescending
                ? query.OrderByDescending(x => x.EmployeeName)
                : query.OrderBy(x => x.EmployeeName),
            "actiontype" => filter.SortDescending
                ? query.OrderByDescending(x => x.ActionType)
                : query.OrderBy(x => x.ActionType),
            "status" => filter.SortDescending
                ? query.OrderByDescending(x => x.Status)
                : query.OrderBy(x => x.Status),
            _ => query.OrderByDescending(x => x.EffectiveDate)
        };

        // ═══════════════════════════════════════════════════════════════
        // CONTAR TOTAL Y PAGINAR
        // ═══════════════════════════════════════════════════════════════

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(ct);

        // ═══════════════════════════════════════════════════════════════
        // CALCULAR AFFECTEDCOUNT PARA ACCIONES MASIVAS
        // ═══════════════════════════════════════════════════════════════

        var batchReferences = items
            .Where(x => !string.IsNullOrEmpty(x.BatchReference))
            .Select(x => x.BatchReference)
            .Distinct()
            .ToList();

        if (batchReferences.Any())
        {
            var batchCounts = await _context.PersonalActions
                .Where(pa => batchReferences.Contains(pa.BatchReference))
                .GroupBy(pa => pa.BatchReference)
                .Select(g => new { BatchRef = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.BatchRef!, x => x.Count, ct);

            foreach (var item in items.Where(x => !string.IsNullOrEmpty(x.BatchReference)))
            {
                if (batchCounts.TryGetValue(item.BatchReference!, out var count))
                {
                    item.AffectedCount = count;
                }
            }
        }

        return new PersonalActionPagedResultDto
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    //// ✅ Método auxiliar para descripciones (ya lo tenemos del handler anterior)
    //private static string GetActionTypeDescription(ActionType actionType) => actionType switch
    //{
    //    ActionType.SalaryChange => "💰 Cambio de Salario",
    //    ActionType.PositionChange => "🔄 Cambio de Puesto",
    //    ActionType.ContractChange => "📄 Cambio de Contrato",
    //    ActionType.ShiftChange => "⏰ Cambio de Turno",
    //    ActionType.CostCenterChange => "🏢 Cambio de Centro de Costo",
    //    ActionType.HealthProviderChange => "🏥 Cambio de Proveedor de Salud",
    //    ActionType.BankAccountChange => "🏦 Cambio de Cuenta Bancaria",
    //    ActionType.Suspension => "⏸️ Suspensión",
    //    ActionType.MassSuspension => "🏭 Suspensión Colectiva",
    //    ActionType.Reinstatement => "▶️ Reincorporación",
    //    ActionType.Termination => "🚪 Terminación",
    //    ActionType.MassTermination => "🏚️ Terminación Masiva",
    //    _ => actionType.ToString()
    //};

}