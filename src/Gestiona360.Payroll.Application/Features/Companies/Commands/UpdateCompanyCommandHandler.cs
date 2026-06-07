using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.Companies.Commands
{
    public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, Unit>
    {
        private readonly ApplicationDbContext _context;

        public UpdateCompanyCommandHandler(ApplicationDbContext context) => _context = context;

        public async Task<Unit> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == request.Request.Id, cancellationToken);

            if (company == null) throw new InvalidOperationException("Empresa no encontrada");

            // Mapeo de campos editables
            company.LegalName = request.Request.LegalName;
            company.CommercialName = request.Request.CommercialName;
            company.TaxId = request.Request.TaxId;
            company.INSSEmployerCode = request.Request.INSSEmployerCode;
            company.EconomicActivityCode = request.Request.EconomicActivityCode;
            company.Phone = request.Request.Phone;
            company.Email = request.Request.Email;
            company.Address = request.Request.Address;
            company.City = request.Request.City;
            company.Department = request.Request.Department;
            company.LogoUrl = request.Request.LogoUrl;
            company.LegalRepresentative = request.Request.LegalRepresentative;
            company.LegalRepresentativeId = request.Request.LegalRepresentativeId;
            company.DefaultCurrency = request.Request.DefaultCurrency;
            company.DefaultPayrollFrequencyId = request.Request.DefaultPayrollFrequencyId;
            company.DefaultIsZoneFranca = request.Request.DefaultIsZoneFranca;
            company.UpdatedAt = DateTime.UtcNow;
       
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
