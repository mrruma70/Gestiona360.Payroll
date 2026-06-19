using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Exceptions;
using Gestiona360.Payroll.Domain.Interfaces;

namespace Gestiona360.Payroll.Domain.Services
{
    public class InatecConfigDomainService
    {
        private readonly IInatecConfigRepository _repository;

        public InatecConfigDomainService(IInatecConfigRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Valida que la nueva vigencia sea posterior a la actual y cierra la configuración vigente.
        /// </summary>
        // ✅ CAMBIO: DateTime → DateOnly
        public async Task<INATECConfig?> ValidateAndCloseCurrentAsync(DateOnly newEffectiveFrom, CancellationToken ct)
        {
            var currentConfig = await _repository.GetCurrentActiveConfigAsync(ct);

            if (currentConfig != null)
            {
                if (newEffectiveFrom <= currentConfig.EffectiveFrom)
                {
                    throw new BusinessRuleViolationException(
                        "La fecha de inicio debe ser posterior a la vigencia actual.");
                }

                currentConfig.EffectiveTo = newEffectiveFrom.AddDays(-1);
                currentConfig.UpdatedAt = DateTime.UtcNow; // ✅ DateTime.UtcNow está bien (UpdatedAt es DateTime?)
            }

            return currentConfig;
        }
    }
}
