using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Shared.Frontend.Enums;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Strategies
{
    public class PositionChangeStrategy : IPersonalActionStrategy
    {
        public ActionType ActionType => ActionType.PositionChange;

        public async Task ExecuteAsync(PersonalAction action, Employee employee, CancellationToken ct)
        {
            if (action.NewJobGradeId.HasValue)
                employee.JobGradeId = action.NewJobGradeId.Value;

            if (action.NewBaseSalary.HasValue)
                employee.BaseSalary = action.NewBaseSalary.Value;

            await Task.CompletedTask;
        }
    }
}
