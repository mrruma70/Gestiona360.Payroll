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

namespace Gestiona360.Payroll.Application.Features.INSSConfigs.Commands
{
    public class CreateNewINSSValidityCommandHandler : IRequestHandler<CreateNewINSSValidityCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly InssConfigDomainService _domainService;

        public CreateNewINSSValidityCommandHandler(
            IUnitOfWork unitOfWork,
            InssConfigDomainService domainService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _domainService = domainService ?? throw new ArgumentNullException(nameof(domainService));
        }

        public async Task<Unit> Handle(CreateNewINSSValidityCommand request, CancellationToken ct)
        {
            // 1. Validar y cerrar configuración actual (si existe)
            var currentConfig = await _domainService.ValidateAndCloseCurrentAsync(request.EffectiveFrom, ct);

            if (currentConfig != null)
            {
                _unitOfWork.InssConfigs.UpdateConfig(currentConfig);
            }

            // 2. Crear nueva configuración
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

            await _unitOfWork.InssConfigs.CreateConfigAsync(newConfig, ct);

            // 3. Persistir (UnitOfWorkBehavior maneja la transacción automáticamente)
            await _unitOfWork.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
