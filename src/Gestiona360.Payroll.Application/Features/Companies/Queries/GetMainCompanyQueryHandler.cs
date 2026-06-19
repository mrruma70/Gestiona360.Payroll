

// src/Gestiona360.Payroll.Application/Features/Companies/Queries/GetMainCompanyQueryHandler.cs

using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Application.Mappers;
using Gestiona360.Payroll.Domain.Interfaces;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.Companies.Queries;

public class GetMainCompanyQueryHandler : IRequestHandler<GetMainCompanyQuery, CompanyDto?>
{
    private readonly ICompanyRepository _companyRepository;

    public GetMainCompanyQueryHandler(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
    }

    public async Task<CompanyDto?> Handle(GetMainCompanyQuery request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetMainCompanyAsync(cancellationToken);

        return company is null ? null : CompanyMapper.ToDto(company);
    }
}