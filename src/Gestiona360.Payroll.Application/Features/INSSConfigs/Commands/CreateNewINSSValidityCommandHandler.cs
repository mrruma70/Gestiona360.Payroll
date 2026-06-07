using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.INSSConfigs.Commands
{
    public class CreateNewINSSValidityCommandHandler : IRequestHandler<CreateNewINSSValidityCommand, Unit>
    {
        private readonly ApplicationDbContext _context;

        public CreateNewINSSValidityCommandHandler(ApplicationDbContext context) => _context = context;

        public async Task<Unit> Handle(CreateNewINSSValidityCommand request, CancellationToken ct)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(ct);
            try
            {
                // 1. Buscar la configuración actualmente VIGENTE
                var currentConfig = await _context.INSSConfigs
                    .Where(c => c.EffectiveTo == null && c.IsActive)
                    .OrderByDescending(c => c.EffectiveFrom)
                    .FirstOrDefaultAsync(ct);

                // 2. Si existe, la cerramos un día antes de que empiece la nueva
                if (currentConfig != null)
                {
                    if (request.EffectiveFrom <= currentConfig.EffectiveFrom)
                        throw new InvalidOperationException("La fecha de inicio debe ser posterior a la vigencia actual.");

                    currentConfig.EffectiveTo = request.EffectiveFrom.AddDays(-1);
                    currentConfig.UpdatedAt = DateTime.UtcNow;
                }

                // 3. Creamos la NUEVA configuración (Vigente hasta nuevo aviso)
                var newConfig = new INSSConfig
                {
                    EffectiveFrom = request.EffectiveFrom,
                    EffectiveTo = null,
                    LegalReference = request.LegalReference,
                    RateWorker = request.RateWorker,
                    RateEmployerSmall = request.RateEmployerSmall,
                    RateEmployerLarge = request.RateEmployerLarge,
                    MaxSalaryCap = request.MaxSalaryCap,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.INSSConfigs.Add(newConfig);
                await _context.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);

                return Unit.Value;
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        }
    }
}
