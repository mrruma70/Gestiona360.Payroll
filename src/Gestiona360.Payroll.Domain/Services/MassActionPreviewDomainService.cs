using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Interfaces;
using Gestiona360.Payroll.Domain.Shared.Frontend;

namespace Gestiona360.Payroll.Domain.Services
{
    public class MassActionPreviewDomainService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IPayrollPeriodRepository _payrollPeriodRepository;
        private readonly IPersonalActionRepository _personalActionRepository;

        public MassActionPreviewDomainService(
            IEmployeeRepository employeeRepository,
            IPayrollPeriodRepository payrollPeriodRepository,
            IPersonalActionRepository personalActionRepository)
        {
            _employeeRepository = employeeRepository;
            _payrollPeriodRepository = payrollPeriodRepository;
            _personalActionRepository = personalActionRepository;
        }

        public async Task<MassActionPreviewDto> GeneratePreviewAsync(
            CreateMassPersonalActionDto dto,
            CancellationToken ct)
        {
            var result = new MassActionPreviewDto
            {
                BatchReference = dto.BatchReference,
                ActionType = dto.ActionType,
                CausalDescription = dto.CausalDescription,
                EffectiveDate = dto.EffectiveDate,
                EndDate = dto.EndDate,
                EmployeesPreview = new List<EmployeeMassPreviewDto>(),
                ValidationErrors = new List<string>()
            };

            // 1. Validar unicidad del BatchReference
            if (await _personalActionRepository.ExistsByBatchReferenceAsync(dto.BatchReference, ct))
            {
                result.ValidationErrors.Add($"La referencia de lote '{dto.BatchReference}' ya existe.");
                return result;
            }

            // 2. Obtener empleados y validar existencia
            var employees = (await _employeeRepository.GetByIdsAsync(dto.TargetEmployeeIds, ct)).ToList();

            if (employees.Count != dto.TargetEmployeeIds.Count)
            {
                result.ValidationErrors.Add("Algunos IDs de empleados no fueron encontrados.");
                return result;
            }

            // 3. Validar grupo de nómina único
            var firstGroupId = employees.First().PayrollGroupId;
            if (!employees.All(e => e.PayrollGroupId == firstGroupId))
            {
                result.ValidationErrors.Add("Todos los empleados deben pertenecer al mismo Grupo de Nómina.");
                return result;
            }

            // 4. Validar período
            var targetPeriod = await _payrollPeriodRepository.GetPeriodByDateAndPayrollGroupAsync(
                firstGroupId, dto.EffectiveDate, ct);

            if (targetPeriod == null || targetPeriod.Status != "Open")
            {
                result.ValidationErrors.Add("No hay un período de nómina abierto para la fecha de efecto seleccionada.");
                return result;
            }

            // 5. Validaciones específicas
            if (dto.ActionType == "MassSuspension" && string.IsNullOrWhiteSpace(dto.MitrabAuthorizationNumber))
            {
                result.ValidationErrors.Add("La autorización MITRAB es obligatoria para suspensiones masivas.");
            }

            if (result.ValidationErrors.Any())
                return result;

            // 6. Calcular preview por empleado
            foreach (var emp in employees)
            {
                result.EmployeesPreview.Add(CalculateEmployeePreview(emp, dto));
            }

            result.TotalEmployees = result.EmployeesPreview.Count;
            return result;
        }

        private EmployeeMassPreviewDto CalculateEmployeePreview(
            Employee emp,
            CreateMassPersonalActionDto dto)
        {
            var previewItem = new EmployeeMassPreviewDto
            {
                EmployeeId = emp.Id,
                Code = emp.Code,
                FullName = $"{emp.FirstName} {emp.LastName}",
                IsValid = true
            };

            try
            {
                if (dto.ActionType == "SalaryChange")
                {
                    previewItem.CurrentValue = $"C$ {emp.BaseSalary:N2}";

                    var newValue = dto.RuleType == "Fixed"
                        ? dto.RuleValue ?? 0
                        : emp.BaseSalary * (1 + (dto.RuleValue ?? 0) / 100);

                    previewItem.NewValue = $"C$ {newValue:N2}";

                    if (newValue < 11350) // TODO: Inyectar servicio real
                    {
                        previewItem.IsValid = false;
                        previewItem.ValidationMessage = "Salario inferior al mínimo legal.";
                    }
                }
                else if (dto.ActionType == "MassSuspension")
                {
                    previewItem.CurrentValue = "Activo";
                    previewItem.NewValue = "Suspendido";
                }
            }
            catch (Exception ex)
            {
                previewItem.IsValid = false;
                previewItem.ValidationMessage = $"Error de cálculo: {ex.Message}";
            }

            return previewItem;
        }
    }
}
