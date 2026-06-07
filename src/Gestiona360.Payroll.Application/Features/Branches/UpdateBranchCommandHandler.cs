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
    public class UpdateBranchCommandHandler : IRequestHandler<UpdateBranchCommand, Unit>
    {
        private readonly ApplicationDbContext _context;

        public UpdateBranchCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateBranchCommand command, CancellationToken cancellationToken)
        {
            var branch = await _context.Branches
                .FirstOrDefaultAsync(b => b.Id == command.Id, cancellationToken);

            if (branch == null)
                throw new KeyNotFoundException($"No se encontró la sucursal con ID '{command.Id}'.");

            var request = command.Request;

            // Validar código único (excluyendo el registro actual)
            var existsCode = await _context.Branches
                .AnyAsync(b => b.Code == request.Code && b.Id != command.Id, cancellationToken);

            if (existsCode)
                throw new InvalidOperationException($"Ya existe otra sucursal con el código '{request.Code}'.");

            // Actualizar campos
            branch.Code = request.Code.ToUpper();
            branch.Name = request.Name.Trim();
            branch.Address = request.Address.Trim();
            branch.City = request.City.Trim();
            branch.Phone = request.Phone.Trim();
            branch.ManagerEmployeeId = request.ManagerEmployeeId;
            branch.DefaultCostCenterId = request.DefaultCostCenterId;
            branch.IsZoneFranca = request.IsZoneFranca;
            branch.IsActive = request.IsActive;
            branch.UpdatedAt = DateTime.UtcNow;

            _context.Branches.Update(branch);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }

}
