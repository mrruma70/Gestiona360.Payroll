using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Interfaces;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using Gestiona360.Payroll.Domain.Shared.Frontend.Enums;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Strategies
{
    public class TerminationStrategy : IPersonalActionStrategy
    {
        public ActionType ActionType => ActionType.Termination;

        public async Task ExecuteAsync(PersonalAction action, Employee employee, CancellationToken ct)
        {
            employee.EmploymentStatus = EmploymentStatus.Terminated;
            employee.TerminationDate = action.EffectiveDate;
            employee.IsActive = false; // si usas IsActive también
            employee.UpdatedAt = DateTime.UtcNow;
            await Task.CompletedTask;
        }
    }
}
