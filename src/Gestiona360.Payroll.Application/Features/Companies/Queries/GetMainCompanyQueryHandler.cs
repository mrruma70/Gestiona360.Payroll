using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.Companies.Queries
{
    public class GetMainCompanyQueryHandler : IRequestHandler<GetMainCompanyQuery, CompanyDto?>
    {
        private readonly ApplicationDbContext _context;

        public GetMainCompanyQueryHandler(ApplicationDbContext context) => _context = context;

        public async Task<CompanyDto?> Handle(GetMainCompanyQuery request, CancellationToken cancellationToken)
        {
            // Obtenemos la primera empresa activa (asumiendo monoempresa o la principal)
            var company = await _context.Companies
                .Where(c => c.IsActive)
                .OrderBy(c => c.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (company == null) return null;

            return new CompanyDto
            {
                Id = company.Id,
                LegalName = company.LegalName,
                CommercialName = company.CommercialName,
                TaxId = company.TaxId,
                INSSEmployerCode = company.INSSEmployerCode,
                EconomicActivityCode = company.EconomicActivityCode,
                Phone = company.Phone,
                Email = company.Email,
                Address = company.Address,
                City = company.City,
                Department = company.Department,
                LogoUrl = company.LogoUrl,
                LegalRepresentative = company.LegalRepresentative,
                LegalRepresentativeId = company.LegalRepresentativeId,
                DefaultCurrency = company.DefaultCurrency,
                DefaultPayrollFrequencyId = company.DefaultPayrollFrequencyId,
                DefaultIsZoneFranca = company.DefaultIsZoneFranca
             
            };
        }
    }
}