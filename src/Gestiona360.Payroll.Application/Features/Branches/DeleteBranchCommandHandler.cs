using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.Branches
{
    public class DeleteBranchCommandHandler : IRequestHandler<DeleteBranchCommand, Unit>
    {
        private readonly ApplicationDbContext _context;

        public DeleteBranchCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteBranchCommand command, CancellationToken cancellationToken)
        {
            var branch = await _context.Branches
                .FirstOrDefaultAsync(b => b.Id == command.Id, cancellationToken);

            if (branch == null)
                throw new KeyNotFoundException($"No se encontró la sucursal con ID '{command.Id}'.");

            // Verificar si tiene empleados asignados
            var hasEmployees = await _context.Employees
                .AnyAsync(e => e.BranchId == command.Id && e.IsActive, cancellationToken);

            if (hasEmployees)
                throw new InvalidOperationException(
                    $"No se puede desactivar la sucursal '{branch.Name}' porque tiene empleados activos asignados.");

            // Baja lógica (no eliminar físicamente)
            branch.IsActive = false;
            branch.UpdatedAt = DateTime.UtcNow;

            _context.Branches.Update(branch);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
