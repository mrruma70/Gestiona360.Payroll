using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Interfaces;
using Gestiona360.Payroll.Domain.Shared.Frontend.Enums;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Strategies
{
    public class ContractChangeStrategy : IPersonalActionStrategy
    {
        public ActionType ActionType => ActionType.ContractChange;

        public async Task ExecuteAsync(PersonalAction action, Employee employee, CancellationToken ct)
        {
            if (action.NewContractTypeId.HasValue)
                employee.ContractTypeId = action.NewContractTypeId.Value;
            await Task.CompletedTask;
        }
    }
}
