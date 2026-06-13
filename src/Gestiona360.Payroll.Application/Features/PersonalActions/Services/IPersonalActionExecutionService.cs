using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Entities;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Services
{
    public interface IPersonalActionExecutionService
    {
        Task ExecuteAsync(
            PersonalAction action,
            Employee employee,
            CancellationToken cancellationToken);
    }
}
