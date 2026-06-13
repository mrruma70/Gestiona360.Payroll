using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using Gestiona360.Payroll.Domain.Shared.Frontend.Enums;
using Gestiona360.Payroll.Infrastructure.Persistence;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Strategies
{
    public class SuspensionStrategy : IPersonalActionStrategy
    {
        private readonly ApplicationDbContext _context;

        public SuspensionStrategy(ApplicationDbContext context)
        {
            _context = context;
        }

        public ActionType ActionType => ActionType.Suspension;

        public async Task ExecuteAsync(PersonalAction action, Employee employee, CancellationToken ct)
        {
            // 1. Actualizar el empleado
            employee.EmploymentStatus = EmploymentStatus.Suspended;
            employee.SuspensionStartDate = action.EffectiveDate;
            employee.SuspensionEndDate = null;
            employee.UpdatedAt = DateTime.UtcNow;

            // 2. Leer los detalles de la suspensión desde DocumentAttachments (JSON)
            var suspensionDetails = JsonSerializer.Deserialize<Dictionary<string, object>>(action.DocumentAttachments);

            var suspensionTypeValue = suspensionDetails != null && suspensionDetails.ContainsKey("SuspensionType")
                ? Convert.ToInt32(suspensionDetails["SuspensionType"])
                : (int)SuspensionType.MedicalLeave;

            var suspensionEndDate = suspensionDetails != null && suspensionDetails.ContainsKey("SuspensionEndDate")
                ? DateTime.Parse(suspensionDetails["SuspensionEndDate"].ToString())
                : (DateTime?)null;

            var mitrabAuth = suspensionDetails?.ContainsKey("MitrabAuthorizationNumber") == true
                ? suspensionDetails["MitrabAuthorizationNumber"].ToString()
                : null;

            // 3. Crear registro en EmployeeSuspensions
            var suspension = new EmployeeSuspension
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee.Id,
                StartDate = action.EffectiveDate,
                EndDate = suspensionEndDate,
                SuspensionType = (SuspensionType)suspensionTypeValue, // ✅ Conversión explícita
                Justification = action.Justification,
                MitrabAuthorizationNumber = mitrabAuth,
                LinkedPersonalActionId = action.Id,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            _context.EmployeeSuspensions.Add(suspension);

            await _context.SaveChangesAsync(ct);
        }
    }
}
