using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using Gestiona360.Payroll.Domain.Shared.Frontend.Enums;
using Gestiona360.Payroll.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Services
{
    public sealed class PersonalActionExecutionService : IPersonalActionExecutionService
    {
        private readonly ApplicationDbContext _context;

        public PersonalActionExecutionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task ExecuteAsync(
            PersonalAction action,
            Employee employee,
            CancellationToken cancellationToken)
        {
            if (action.Status == ActionStatus.Executed)
                return;

            action.Status = ActionStatus.Executed;
            action.ExecutedDate = DateTime.UtcNow;

            switch (action.ActionType)
            {
                case ActionType.SalaryChange:

                    if (action.NewBaseSalary.HasValue)
                        employee.BaseSalary = action.NewBaseSalary.Value;

                    break;

                case ActionType.PositionChange:

                    if (action.NewJobGradeId.HasValue)
                        employee.JobGradeId = action.NewJobGradeId.Value;

                    break;

                case ActionType.ContractChange:

                    if (action.NewContractTypeId.HasValue)
                        employee.ContractTypeId = action.NewContractTypeId.Value;

                    break;

                case ActionType.CostCenterChange:

                    if (action.NewCostCenterId.HasValue)
                        employee.CostCenterId = action.NewCostCenterId.Value;

                    break;

                case ActionType.ShiftChange:

                    if (action.NewShiftId.HasValue)
                    {
                        var currentShift =
                            await _context.EmployeeShiftAssignments
                                .FirstOrDefaultAsync(
                                    x => x.EmployeeId == employee.Id &&
                                         x.EndDate == null,
                                    cancellationToken);

                        if (currentShift != null)
                            currentShift.EndDate =
                                action.EffectiveDate.AddDays(-1);

                        _context.EmployeeShiftAssignments.Add(
                            new EmployeeShiftAssignment
                            {
                                Id = Guid.NewGuid(),
                                EmployeeId = employee.Id,
                                ShiftId = action.NewShiftId.Value,
                                StartDate = action.EffectiveDate,
                                LinkedToPersonalActionId = action.Id
                            });
                    }

                    break;

                case ActionType.Suspension:
                case ActionType.MassSuspension:

                    employee.EmploymentStatus =
                        EmploymentStatus.Suspended;

                    _context.EmployeeSuspensions.Add(
                        new EmployeeSuspension
                        {
                            Id = Guid.NewGuid(),
                            EmployeeId = employee.Id,
                            StartDate = action.EffectiveDate,
                            LinkedPersonalActionId = action.Id
                        });

                    break;

                case ActionType.Termination:
                case ActionType.MassTermination:

                    employee.EmploymentStatus =
                        EmploymentStatus.Terminated;

                    employee.TerminationDate =
                        action.EffectiveDate;

                    break;
            }

            _context.Employees.Update(employee);
        }
    }
}
