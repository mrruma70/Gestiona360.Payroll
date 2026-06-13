using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Application.Contracts;
using Gestiona360.Payroll.Application.Contracts.DTOs.PersonalActions;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Infrastructure.Services
{
    public class PersonalActionImpactAnalyzer : IPersonalActionImpactAnalyzer
    {
        private readonly IApplicationDbContext _context;

        public PersonalActionImpactAnalyzer(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PersonalActionImpactResult> AnalyzeAsync(Guid employeeId, DateTime effectiveDate)
        {
            var today = DateTime.Today;
            var result = new PersonalActionImpactResult();

            // 1. Determinar Temporalidad
            if (effectiveDate.Date < today.Date)
            {
                result.IsRetroactive = true;
                result.UserMessage = "⚠️ Esta acción es RETROACTIVA. Puede requerir ajustes de nómina.";
            }
            else if (effectiveDate.Date > today.Date)
            {
                result.IsFutureAction = true;
                result.UserMessage = "📅 Esta acción es FUTURA. Se ejecutará automáticamente en la fecha indicada.";
            }
            else
            {
                result.UserMessage = "✅ Acción inmediata.";
            }

            // 2. Identificar Período Afectado
            var period = await _context.PayrollPeriods
                .FirstOrDefaultAsync(p => effectiveDate >= p.StartDate && effectiveDate <= p.EndDate);

            if (period != null)
            {
                result.AffectedPayrollPeriodName = $"Período {period.PeriodNumber} ({period.StartDate:dd/MM} - {period.EndDate:dd/MM})";

                result.PeriodStatus = period.Status; // Open, Closed, Paid

                if (period.Status == "Paid")
                {
                    result.RequiresApproval = true;
                    result.UserMessage += " El período ya está PAGADO. Se generará una Nómina Extraordinaria.";
                }
                else if (period.Status == "Closed")
                {
                    result.RequiresApproval = true;
                    result.UserMessage += " El período está CERRADO. Requiere reapertura o ajuste.";
                }
            }
            else
            {
                result.UserMessage += " No se encontró un período de nómina activo para esta fecha.";
            }

            return result;
        }
    }

}
