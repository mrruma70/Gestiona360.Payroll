using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Application.Abstractions.Repositories;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Services;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.Branches
{
    public class CreateBranchCommandHandler : IRequestHandler<CreateBranchCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly BranchDomainService _domainService;

        public CreateBranchCommandHandler(
            IUnitOfWork unitOfWork,
            BranchDomainService domainService)
        {
            _unitOfWork = unitOfWork;
            _domainService = domainService;
        }

        public async Task<Guid> Handle(CreateBranchCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            // 1. Validar código único
            await _domainService.ValidateCodeIsUniqueAsync(request.Code, cancellationToken);

            // 2. Crear entidad
            var branch = new Branch
            {
                Id = Guid.NewGuid(),
                Code = request.Code.ToUpper().Trim(),
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

            // 3. Persistir
            await _unitOfWork.Branches.AddAsync(branch, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return branch.Id;
        }

    }

}
