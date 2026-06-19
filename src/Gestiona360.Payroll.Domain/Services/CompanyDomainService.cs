using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Exceptions;
using Gestiona360.Payroll.Domain.Interfaces;

namespace Gestiona360.Payroll.Domain.Services
{
    public class CompanyDomainService
    {
        private readonly ICompanyRepository _companyRepository;

        public CompanyDomainService(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }

        /// <summary>
        /// Valida que el RUC/NIT sea único (excluyendo la empresa actual).
        /// </summary>
        public async Task ValidateTaxIdIsUniqueAsync(string taxId, Guid companyId, CancellationToken ct)
        {
            var exists = await _companyRepository.ExistsWithTaxIdAsync(taxId, companyId, ct);
            if (exists)
            {
                throw new BusinessRuleViolationException(
                    $"Ya existe otra empresa con el RUC/NIT '{taxId}'.");
            }
        }
    }
}
