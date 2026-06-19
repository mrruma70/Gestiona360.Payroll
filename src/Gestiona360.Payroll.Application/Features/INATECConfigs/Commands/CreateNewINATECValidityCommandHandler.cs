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

namespace Gestiona360.Payroll.Application.Features.INATECConfigs.Commands
{
    public class CreateNewINATECValidityCommandHandler : IRequestHandler<CreateNewINATECValidityCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly InatecConfigDomainService _domainService;

        public CreateNewINATECValidityCommandHandler(
            IUnitOfWork unitOfWork,
            InatecConfigDomainService domainService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _domainService = domainService ?? throw new ArgumentNullException(nameof(domainService));
        }

        public async Task<Unit> Handle(CreateNewINATECValidityCommand request, CancellationToken ct)
        {
            // 1. Validar y cerrar configuración actual (si existe)
            var currentConfig = await _domainService.ValidateAndCloseCurrentAsync(request.EffectiveFrom, ct);

            if (currentConfig != null)
            {
                _unitOfWork.InatecConfigs.UpdateConfig(currentConfig);
            }

            // 2. Crear nueva configuración
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

            await _unitOfWork.InatecConfigs.CreateConfigAsync(newConfig, ct);

            // 3. Persistir
            await _unitOfWork.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
