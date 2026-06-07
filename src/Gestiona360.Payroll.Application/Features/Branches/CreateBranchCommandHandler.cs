using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.Branches
{
    public class CreateBranchCommandHandler : IRequestHandler<CreateBranchCommand, Guid>
    {
        private readonly ApplicationDbContext _context;

        public CreateBranchCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateBranchCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            // Validar código único
            var existsCode = await _context.Branches
                .AnyAsync(b => b.Code == request.Code, cancellationToken);

            if (existsCode)
                throw new InvalidOperationException($"Ya existe una sucursal con el código '{request.Code}'.");

            // Validar que la empresa existe
            var companyExists = await _context.Companies
                .AnyAsync(c => c.Id == request.CompanyId, cancellationToken);

            if (!companyExists)
                throw new InvalidOperationException($"La empresa con ID '{request.CompanyId}' no existe.");

            var branch = new Branch
            {
                Id = Guid.NewGuid(),
                Code = request.Code.ToUpper(),
                Name = request.Name.Trim(),
                Address = request.Address.Trim(),
                City = request.City.Trim(),
                Phone = request.Phone.Trim(),
                CompanyId = request.CompanyId,
                ManagerEmployeeId = request.ManagerEmployeeId,
                DefaultCostCenterId = request.DefaultCostCenterId,
                IsZoneFranca = request.IsZoneFranca,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Branches.Add(branch);
            await _context.SaveChangesAsync(cancellationToken);

            return branch.Id;
        }
    }

}
