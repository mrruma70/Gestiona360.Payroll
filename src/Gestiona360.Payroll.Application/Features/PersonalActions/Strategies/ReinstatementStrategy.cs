using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Application.Contracts;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using Gestiona360.Payroll.Domain.Shared.Frontend.Enums;
using Gestiona360.Payroll.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Strategies
{
    public class ReinstatementStrategy : IPersonalActionStrategy
    {
        private readonly ApplicationDbContext _context;

        public ReinstatementStrategy(ApplicationDbContext context)
        {
            _context = context;
        }

        public ActionType ActionType => ActionType.Reinstatement;

        public async Task ExecuteAsync(PersonalAction action, Employee employee, CancellationToken ct)
        {
            // 1. Revertir el estado del empleado
            employee.EmploymentStatus = EmploymentStatus.Active;
            employee.SuspensionStartDate = null;
            employee.SuspensionEndDate = null;
            employee.UpdatedAt = DateTime.UtcNow;

            // 2. Cerrar la suspensión activa en EmployeeSuspensions
            var activeSuspension = await _context.EmployeeSuspensions
                .FirstOrDefaultAsync(s => s.EmployeeId == employee.Id && s.EndDate == null, ct);
            if (activeSuspension != null)
            {
                activeSuspension.EndDate = action.EffectiveDate;
                activeSuspension.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(ct);
        }
    }
}
