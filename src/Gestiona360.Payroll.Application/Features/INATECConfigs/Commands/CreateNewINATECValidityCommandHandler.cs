using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.INATECConfigs.Commands
{
    public class CreateNewINATECValidityCommandHandler : IRequestHandler<CreateNewINATECValidityCommand, Unit>
    {
        private readonly ApplicationDbContext _context;

        public CreateNewINATECValidityCommandHandler(ApplicationDbContext context) => _context = context;

        public async Task<Unit> Handle(CreateNewINATECValidityCommand request, CancellationToken ct)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(ct);
            try
            {
                var currentConfig = await _context.INATECConfigs
                    .Where(c => c.EffectiveTo == null && c.IsActive)
                    .OrderByDescending(c => c.EffectiveFrom)
                    .FirstOrDefaultAsync(ct);

                if (currentConfig != null)
                {
                    if (request.EffectiveFrom <= currentConfig.EffectiveFrom)
                        throw new InvalidOperationException("La fecha de inicio debe ser posterior a la vigencia actual.");

                    currentConfig.EffectiveTo = request.EffectiveFrom.AddDays(-1);
                    currentConfig.UpdatedAt = DateTime.UtcNow;
                }

                var newConfig = new INATECConfig
                {
                    EffectiveFrom = request.EffectiveFrom,
                    EffectiveTo = null,
                    LegalReference = request.LegalReference,
                    Rate = request.Rate,
                    Exceptions = request.Exceptions,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.INATECConfigs.Add(newConfig);
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
