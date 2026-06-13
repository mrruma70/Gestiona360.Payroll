using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Shared.Frontend.Enums;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Commands
{
    public class DeletePersonalActionCommandHandler : IRequestHandler<DeletePersonalActionCommand, Unit>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DeletePersonalActionCommandHandler> _logger;

        public DeletePersonalActionCommandHandler(
            ApplicationDbContext context,
            ILogger<DeletePersonalActionCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeletePersonalActionCommand request, CancellationToken cancellationToken)
        {
            var action = await _context.PersonalActions
                .FirstOrDefaultAsync(a => a.Id == request.ActionId, cancellationToken)
                ?? throw new KeyNotFoundException($"Acción de personal con ID {request.ActionId} no encontrada.");

            // 1. Regla de Inmutabilidad Estricta
            if (action.Status == ActionStatus.Executed)
                throw new InvalidOperationException("Acción inmutable: No se puede eliminar una acción que ya ha sido ejecutada. Debe crear una acción correctiva.");

            if (action.Status == ActionStatus.Rejected)
                throw new InvalidOperationException("No se puede eliminar una acción rechazada. El registro debe permanecer como evidencia de auditoría.");

            // 2. Manejo de Acciones Masivas
            if (!string.IsNullOrEmpty(action.BatchReference))
            {
                _logger.LogWarning("Intento de eliminar una acción que pertenece a un lote masivo. BatchReference: {BatchRef}. Se eliminarán todas las acciones pendientes de este lote.", action.BatchReference);

                // Eliminar TODAS las acciones del lote que estén en estado Pending para mantener la integridad del lote
                var batchActionsToDelete = await _context.PersonalActions
                    .Where(a => a.BatchReference == action.BatchReference && a.Status == ActionStatus.Pending)
                    .ToListAsync(cancellationToken);

                _context.PersonalActions.RemoveRange(batchActionsToDelete);
                _logger.LogInformation("Se eliminaron {Count} acciones del lote {BatchRef}", batchActionsToDelete.Count, action.BatchReference);
            }
            else
            {
                // 3. Eliminación individual
                _context.PersonalActions.Remove(action);
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Acción de personal eliminada exitosamente. Id: {ActionId}", request.ActionId);
            return Unit.Value;
        }
    }
}
