using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Application.Abstractions.Repositories;
using Gestiona360.Payroll.Domain.Exceptions;
using Gestiona360.Payroll.Domain.Services;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.Companies.Commands
{

    public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly CompanyDomainService _domainService;

        public UpdateCompanyCommandHandler(
            IUnitOfWork unitOfWork,
            CompanyDomainService domainService)
        {
            _unitOfWork = unitOfWork;
            _domainService = domainService;
        }

        public async Task<Unit> Handle(UpdateCompanyCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            // 1. Obtener empresa
            var company = await _unitOfWork.Companies.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException("Company", request.Id);

            // 2. Validar RUC único (si cambió)
            if (company.TaxId != request.TaxId)
            {
                await _domainService.ValidateTaxIdIsUniqueAsync(request.TaxId, request.Id, cancellationToken);
            }

            // 3. Actualizar campos
            company.LegalName = request.LegalName.Trim();
            company.CommercialName = request.CommercialName?.Trim();
            company.TaxId = request.TaxId.Trim();
            company.INSSEmployerCode = request.INSSEmployerCode?.Trim();
            company.EconomicActivityCode = request.EconomicActivityCode?.Trim();
            company.Phone = request.Phone?.Trim();
            company.Email = request.Email?.Trim();
            company.Address = request.Address?.Trim();
            company.City = request.City?.Trim();
            company.Department = request.Department?.Trim();
            company.LogoUrl = request.LogoUrl;
            company.LegalRepresentative = request.LegalRepresentative?.Trim();
            company.LegalRepresentativeId = request.LegalRepresentativeId;
            company.DefaultCurrency = request.DefaultCurrency;
            company.DefaultPayrollFrequencyId = request.DefaultPayrollFrequencyId;
            company.DefaultIsZoneFranca = request.DefaultIsZoneFranca;
            company.UpdatedAt = DateTime.UtcNow;

            // 4. Persistir
            _unitOfWork.Companies.Update(company);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }

}
