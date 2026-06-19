using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Exceptions;
using Gestiona360.Payroll.Domain.Interfaces;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using Gestiona360.Payroll.Domain.Shared.Frontend.Enums;

namespace Gestiona360.Payroll.Domain.Services
{
    public class PersonalActionDomainService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IPayrollPeriodRepository _payrollPeriodRepository;
        private readonly IEmployeeShiftAssignmentRepository _shiftAssignmentRepository;
        private readonly IPersonalActionRepository _repository;

        public PersonalActionDomainService(
       IEmployeeRepository employeeRepository,
       IPayrollPeriodRepository payrollPeriodRepository,
       IEmployeeShiftAssignmentRepository shiftAssignmentRepository,
       IPersonalActionRepository personalActionRepository)
        {
            _employeeRepository = employeeRepository;
            _payrollPeriodRepository = payrollPeriodRepository;
            _shiftAssignmentRepository = shiftAssignmentRepository;
            _repository = personalActionRepository;
        }

        /// <summary>
        /// Crea las acciones masivas aplicando las reglas de negocio.
        /// </summary>
        public async Task<List<PersonalAction>> CreateMassActionsAsync(
            MassPersonalActionData data,
            CancellationToken ct)
        {
            // 1. Obtener empleados
            var employees = (await _repository.GetEmployeesByIdsAsync(data.TargetEmployeeIds, ct)).ToList();

            if (!employees.Any())
                throw new BusinessRuleViolationException("No se encontraron empleados para la acción masiva.");

            // 2. Obtener período objetivo (basado en el primer empleado)
            var firstEmployee = employees.First();
            var targetPeriod = await _repository.GetPeriodByDateAndPayrollGroupAsync(
                firstEmployee.PayrollGroupId,
                data.EffectiveDate,
                ct);

            if (targetPeriod == null)
                throw new BusinessRuleViolationException(
                    $"No se encontró un período de nómina para la fecha {data.EffectiveDate:yyyy-MM-dd}.");

            // 3. Crear acciones
            var actions = new List<PersonalAction>();

            foreach (var emp in employees)
            {
                var (newBaseSalary, newStatus) = CalculateNewValues(data, emp);

                var action = new PersonalAction
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = emp.Id,
                    PayrollGroupId = emp.PayrollGroupId,
                    TargetPayrollPeriodId = targetPeriod.Id,
                    ActionType = Enum.Parse<ActionType>(data.ActionType),
                    EffectiveDate = data.EffectiveDate,
                    Status = ActionStatus.Executed,
                    CausalDescription = data.CausalDescription,
                    Justification = data.Justification,
                    BatchReference = data.BatchReference,
                    IsAppliedInPayroll = false,

                    // Old Values
                    OldBaseSalary = emp.BaseSalary,
                    OldEmploymentStatus = (int)emp.EmploymentStatus,

                    // New Values
                    NewBaseSalary = newBaseSalary,
                    NewEmploymentStatus = newStatus,

                    DocumentAttachments = System.Text.Json.JsonSerializer.Serialize(data.MasterDocuments),
                    CreatedAt = DateTime.UtcNow
                };

                actions.Add(action);
            }

            return actions;
        }

        /// <summary>
        /// Calcula los nuevos valores según el tipo de acción y regla.
        /// </summary>
        private static (decimal? newBaseSalary, int? newStatus) CalculateNewValues(
            MassPersonalActionData data,
            Employee employee)
        {
            decimal? newBaseSalary = null;
            int? newStatus = null;

            if (data.ActionType == "SalaryChange")
            {
                newBaseSalary = data.RuleType == "Fixed"
                    ? data.RuleValue
                    : employee.BaseSalary * (1 + (data.RuleValue ?? 0) / 100);
            }
            else if (data.ActionType == "MassSuspension")
            {
                newStatus = (int)EmploymentStatus.Suspended;
            }

            return (newBaseSalary, newStatus);
        }


        /// <summary>
        /// Valida y crea una acción de personal individual.
        /// </summary>
        public async Task<PersonalAction> CreatePersonalActionAsync(
            PersonalActionCreationData data,
            CancellationToken ct)
        {
            // 1. Obtener empleado y validar
            var employee = await _employeeRepository.GetByIdAsync(data.EmployeeId, ct)
                ?? throw new NotFoundException("Empleado", data.EmployeeId);

            if (employee.PayrollGroupId == Guid.Empty)
                throw new BusinessRuleViolationException("El empleado no tiene un Grupo de Nómina asignado.");

            // 2. Determinar período objetivo
            var targetPeriod = await _payrollPeriodRepository.GetPeriodByDateAndPayrollGroupAsync(
                employee.PayrollGroupId,
                data.EffectiveDate,
                ct);

            if (targetPeriod == null)
                throw new BusinessRuleViolationException(
                    $"No existe un período de nómina abierto o configurado para la fecha {data.EffectiveDate:dd/MM/yyyy} en el grupo del empleado.");

            // 3. Validar que el período esté "Open"
            if (targetPeriod.Status != "Open")
                throw new BusinessRuleViolationException(
                    $"No se pueden crear acciones porque el período '{targetPeriod.PeriodNumber}' está en estado '{targetPeriod.Status}'.");

            // 4. Validaciones de negocio
            ValidateBusinessRules(data);

            // 5. Obtener turno activo
            var activeShift = await _shiftAssignmentRepository.GetActiveShiftByEmployeeIdAsync(
                employee.Id, ct);

            // 6. Crear la acción
            var action = BuildPersonalAction(data, employee, targetPeriod.Id, activeShift?.ShiftId);

            // 7. Persistir
            await _repository.AddAsync(action, ct);

            return action;
        }

        /// <summary>
        /// Valida las reglas de negocio para la creación de acción.
        /// </summary>
        private static void ValidateBusinessRules(PersonalActionCreationData data)
        {
            // Validar justificación
            if (string.IsNullOrWhiteSpace(data.Justification) || data.Justification.Length < 20)
                throw new BusinessRuleViolationException("La justificación debe tener al menos 20 caracteres.");

            // Validar salario (si aplica)
            if (data.ActionType == "SalaryChange" && data.NewBaseSalary.HasValue)
            {
                if (data.NewBaseSalary.Value <= 0)
                    throw new BusinessRuleViolationException("El nuevo salario debe ser mayor a cero.");
            }
        }

        /// <summary>
        /// Construye la entidad PersonalAction con todos los valores.
        /// </summary>
        private static PersonalAction BuildPersonalAction(
            PersonalActionCreationData data,
            Employee employee,
            Guid targetPayrollPeriodId,
            Guid? oldShiftId)
        {
            var action = new PersonalAction
            {
                Id = Guid.NewGuid(),
                EmployeeId = data.EmployeeId,
                PayrollGroupId = employee.PayrollGroupId,
                TargetPayrollPeriodId = targetPayrollPeriodId,
                ActionType = Enum.Parse<ActionType>(data.ActionType),
                EffectiveDate = data.EffectiveDate,
                Status = data.ExecuteImmediately ? ActionStatus.Executed : ActionStatus.Pending,
                CausalDescription = data.CausalDescription,
                Justification = data.Justification,
                OldShiftId = oldShiftId,
                IsAppliedInPayroll = false,

                // OldValues
                OldBaseSalary = employee.BaseSalary,
                OldJobGradeId = employee.JobGradeId,
                OldContractTypeId = employee.ContractTypeId,
                OldCostCenterId = employee.CostCenterId,
                OldEmploymentStatus = (int)employee.EmploymentStatus,
                OldBankId = employee.BankId,
                OldBankAccountNumber = employee.BankAccountNumber,
                OldHealthProviderId = employee.HealthProviderId,

                // NewValues
                NewBaseSalary = data.NewBaseSalary,
                NewJobGradeId = data.NewJobGradeId,
                NewContractTypeId = data.NewContractTypeId,
                NewCostCenterId = data.NewCostCenterId,
                NewShiftId = data.NewShiftId,
                NewBankId = data.NewBankId,
                NewBankAccountNumber = data.NewBankAccountNumber,
                NewHealthProviderId = data.NewHealthProviderId,

                CreatedAt = DateTime.UtcNow
            };

            // Lógica especial para Suspension
            if (data.ActionType == "Suspension" || data.ActionType == "MassSuspension")
            {
                action.NewEmploymentStatus = (int)EmploymentStatus.Suspended;
            }

            // ✅ CORREGIDO: Usar DocumentAttachmentDto directamente
            if (data.ActionType == "BankAccountChange")
            {
                var extraData = new Dictionary<string, object>
                {
                    ["NewBankId"] = data.NewBankId ?? 0,
                    ["NewBankAccountNumber"] = data.NewBankAccountNumber ?? ""
                };
                var existingDocs = data.Documents ?? new List<DocumentAttachmentDto>();
                var combined = new { documents = existingDocs, actionData = extraData };
                action.DocumentAttachments = JsonSerializer.Serialize(combined);
            }
            else
            {
                action.DocumentAttachments = data.Documents != null && data.Documents.Any()
                    ? JsonSerializer.Serialize(data.Documents)
                    : "[]";
            }

            return action;
        }

        /// <summary>
        /// Elimina una acción de personal aplicando las reglas de negocio.
        /// Si la acción pertenece a un lote, elimina todas las acciones pendientes del lote.
        /// </summary>
        /// <returns>La cantidad de acciones eliminadas.</returns>
        public async Task<int> DeleteActionAsync(Guid actionId, CancellationToken ct)
        {
            // 1. Obtener la acción
            var action = await _repository.GetByIdAsync(actionId, ct)
                ?? throw new NotFoundException("Acción de personal", actionId);

            // 2. Regla de Inmutabilidad Estricta
            if (action.Status == ActionStatus.Executed)
            {
                throw new BusinessRuleViolationException(
                    "Acción inmutable: No se puede eliminar una acción que ya ha sido ejecutada. Debe crear una acción correctiva.");
            }

            if (action.Status == ActionStatus.Rejected)
            {
                throw new BusinessRuleViolationException(
                    "No se puede eliminar una acción rechazada. El registro debe permanecer como evidencia de auditoría.");
            }

            // 3. Manejo de Acciones Masivas
            if (!string.IsNullOrEmpty(action.BatchReference))
            {
                var batchActionsToDelete = await _repository.GetPendingByBatchReferenceAsync(action.BatchReference, ct);

                var batchList = batchActionsToDelete.ToList();
                _repository.DeleteRange(batchList);

                return batchList.Count;
            }

            // 4. Eliminación individual
            _repository.Delete(action);
            return 1;
        }

        /// <summary>
        /// Ejecuta una acción de personal usando la estrategia correspondiente.
        /// </summary>
        public async Task ExecuteActionAsync(
            Guid actionId,
            IEnumerable<IPersonalActionStrategy> strategies,
            CancellationToken ct)
        {
            // 1. Obtener acción con datos del empleado
            var action = await _repository.GetByIdWithEmployeeAsync(actionId, ct)
                ?? throw new NotFoundException("Acción de personal", actionId);

            if (action.Employee == null)
                throw new BusinessRuleViolationException(
                    "No se puede ejecutar la acción porque el empleado asociado no existe.");

            // 2. Validar que la acción esté pendiente
            if (action.Status != ActionStatus.Pending)
            {
                throw new BusinessRuleViolationException(
                    $"No se puede ejecutar una acción en estado '{action.Status}'. Solo las acciones pendientes pueden ejecutarse.");
            }

            // 3. Elegir la estrategia
            var strategy = strategies.FirstOrDefault(s => s.ActionType == action.ActionType)
                ?? throw new BusinessRuleViolationException(
                    $"No hay estrategia registrada para el tipo de acción '{action.ActionType}'.");

            // 4. Ejecutar la estrategia
            await strategy.ExecuteAsync(action, action.Employee, ct);

            // 5. Marcar como ejecutada
            action.Status = ActionStatus.Executed;
            action.ExecutedDate = DateTime.UtcNow;

            _repository.Update(action);
        }

    }
}
