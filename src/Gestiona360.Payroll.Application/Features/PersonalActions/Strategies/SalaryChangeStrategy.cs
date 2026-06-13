using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Shared.Frontend.Enums;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Strategies
{
    public class SalaryChangeStrategy : IPersonalActionStrategy
    {
        public ActionType ActionType => ActionType.SalaryChange;
        public async Task ExecuteAsync(PersonalAction action, Employee employee, CancellationToken ct)
        {
            if (action.NewBaseSalary.HasValue)
                employee.BaseSalary = action.NewBaseSalary.Value;
            await Task.CompletedTask;
        }
    }
}
