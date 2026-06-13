using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Application.Contracts.DTOs.PersonalActions;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Commands
{
    public class GetMassActionPreviewQueryHandler : IRequestHandler<GetMassActionPreviewQuery, MassActionPreviewDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetMassActionPreviewQueryHandler> _logger;

        public GetMassActionPreviewQueryHandler(ApplicationDbContext context, ILogger<GetMassActionPreviewQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<MassActionPreviewDto> Handle(GetMassActionPreviewQuery request, CancellationToken cancellationToken)
        {
            var dto = request.Data;
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
            if (await _context.PersonalActions.AnyAsync(a => a.BatchReference == dto.BatchReference, cancellationToken))
            {
                result.ValidationErrors.Add($"La referencia de lote '{dto.BatchReference}' ya existe.");
                return result;
            }

            // 2. Obtener empleados y validar grupo
            var employees = await _context.Employees
                .Where(e => dto.TargetEmployeeIds.Contains(e.Id))
                .ToListAsync(cancellationToken);

            if (employees.Count != dto.TargetEmployeeIds.Count)
            {
                result.ValidationErrors.Add("Algunos IDs de empleados no fueron encontrados.");
                return result;
            }

            var firstGroupId = employees.FirstOrDefault()?.PayrollGroupId;
            if (!employees.All(e => e.PayrollGroupId == firstGroupId))
            {
                result.ValidationErrors.Add("Todos los empleados deben pertenecer al mismo Grupo de Nómina.");
                return result;
            }

            // 3. Validar Período
            var targetPeriod = await _context.PayrollPeriods
                .FirstOrDefaultAsync(p => p.PayrollGroupId == firstGroupId &&
                                          dto.EffectiveDate >= p.StartDate &&
                                          dto.EffectiveDate <= p.EndDate, cancellationToken);

            if (targetPeriod == null || targetPeriod.Status != "Open")
            {
                result.ValidationErrors.Add("No hay un período de nómina abierto para la fecha de efecto seleccionada.");
                return result;
            }

            // 4. Validaciones específicas (MITRAB)
            if (dto.ActionType == "MassSuspension" && string.IsNullOrWhiteSpace(dto.MitrabAuthorizationNumber))
            {
                result.ValidationErrors.Add("La autorización MITRAB es obligatoria para suspensiones masivas.");
            }

            // 5. Calcular y Validar cada empleado
            foreach (var emp in employees)
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
                    // Lógica de cálculo según ActionType
                    decimal newValue = 0;
                    string currentValueStr = string.Empty;

                    if (dto.ActionType == "SalaryChange")
                    {
                        currentValueStr = $"C$ {emp.BaseSalary:N2}";

                        if (dto.RuleType == "Fixed")
                            newValue = dto.RuleValue ?? 0;
                        else if (dto.RuleType == "Percentage")
                            newValue = emp.BaseSalary * (1 + (dto.RuleValue ?? 0) / 100);

                        previewItem.NewValue = $"C$ {newValue:N2}";

                        // Validación Salario Mínimo (Simplificada)
                        // TODO: Inyectar servicio de salarios mínimos reales
                        if (newValue < 11350) // Ejemplo estático
                        {
                            previewItem.IsValid = false;
                            previewItem.ValidationMessage = "Salario inferior al mínimo legal.";
                        }
                    }
                    else if (dto.ActionType == "MassSuspension")
                    {
                        currentValueStr = "Activo";
                        previewItem.NewValue = "Suspendido";
                    }

                    previewItem.CurrentValue = currentValueStr;
                }
                catch (Exception ex)
                {
                    previewItem.IsValid = false;
                    previewItem.ValidationMessage = $"Error de cálculo: {ex.Message}";
                }

                result.EmployeesPreview.Add(previewItem);
            }

            result.TotalEmployees = result.EmployeesPreview.Count;
            return result;
        }
    }
}
