using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Shared.Frontend.Enums;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Strategies
{
    public interface IPersonalActionStrategy
    {
        ActionType ActionType { get; }
        Task ExecuteAsync(PersonalAction action, Employee employee, CancellationToken ct);
    }
}
