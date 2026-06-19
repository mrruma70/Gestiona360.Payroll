using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Interfaces;
using Gestiona360.Payroll.Domain.Shared.Frontend.Enums;
using Gestiona360.Payroll.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Strategies
{
    public class ShiftChangeStrategy : IPersonalActionStrategy
    {
        private readonly ApplicationDbContext _context;

        public ShiftChangeStrategy(ApplicationDbContext context)
        {
            _context = context;
        }

        public ActionType ActionType => ActionType.ShiftChange;

        public async Task ExecuteAsync(PersonalAction action, Employee employee, CancellationToken ct)
        {
            // 1. Cerrar la asignación actual (si existe)
            if (action.OldShiftId.HasValue)
            {
                var currentAssignment = await _context.EmployeeShiftAssignments
                    .FirstOrDefaultAsync(a => a.EmployeeId == employee.Id && a.EndDate == null, ct);
                if (currentAssignment != null)
                {
                    currentAssignment.EndDate = action.EffectiveDate.AddDays(-1);
                    currentAssignment.UpdatedAt = DateTime.UtcNow;
                }
            }

            // 2. Crear la nueva asignación
            var newAssignment = new EmployeeShiftAssignment
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee.Id,
                ShiftId = action.NewShiftId!.Value,
                StartDate = action.EffectiveDate,
                EndDate = null,
                LinkedToPersonalActionId = action.Id,
                Justification = action.Justification,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            _context.EmployeeShiftAssignments.Add(newAssignment);

            // Nota: No se actualiza ninguna columna en Employees (el empleado no tiene FK directa a Shift)
            await _context.SaveChangesAsync(ct);
        }
    }
}
