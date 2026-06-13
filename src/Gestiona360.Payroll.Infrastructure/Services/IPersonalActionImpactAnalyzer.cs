using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Application.Contracts.DTOs.PersonalActions;

namespace Gestiona360.Payroll.Infrastructure.Services
{
    public interface IPersonalActionImpactAnalyzer
    {
        Task<PersonalActionImpactResult> AnalyzeAsync(Guid employeeId, DateTime effectiveDate);
    }
}
