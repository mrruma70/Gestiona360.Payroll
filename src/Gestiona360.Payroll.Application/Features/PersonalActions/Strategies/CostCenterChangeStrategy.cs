using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Shared.Frontend.Enums;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Strategies
{
    public class CostCenterChangeStrategy : IPersonalActionStrategy
    {
        public ActionType ActionType => ActionType.CostCenterChange;

        public async Task ExecuteAsync(PersonalAction action, Employee employee, CancellationToken ct)
        {
            if (action.NewCostCenterId.HasValue)
                employee.CostCenterId = action.NewCostCenterId.Value;
            await Task.CompletedTask;
        }
    }
}
