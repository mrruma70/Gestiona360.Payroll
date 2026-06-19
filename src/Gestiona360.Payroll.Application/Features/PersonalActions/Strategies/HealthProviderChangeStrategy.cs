using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Interfaces;
using Gestiona360.Payroll.Domain.Shared.Frontend.Enums;
using Gestiona360.Payroll.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Strategies
{
    public class HealthProviderChangeStrategy : IPersonalActionStrategy
    {
        private readonly ApplicationDbContext _context;

        public HealthProviderChangeStrategy(ApplicationDbContext context)
        {
            _context = context;
        }

        public ActionType ActionType => ActionType.HealthProviderChange;

        public async Task ExecuteAsync(PersonalAction action, Employee employee, CancellationToken ct)
        {
            if (action.NewHealthProviderId.HasValue)
            {
                var providerExists = await _context.HealthProviders.AnyAsync(h => h.Id == action.NewHealthProviderId.Value && h.IsActive, ct);
                if (providerExists)
                    employee.HealthProviderId = action.NewHealthProviderId.Value;
                else
                    throw new InvalidOperationException($"El proveedor de salud con ID {action.NewHealthProviderId.Value} no existe o está inactivo.");
            }

            employee.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
        }
    }
}
