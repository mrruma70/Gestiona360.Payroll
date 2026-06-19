
// src/Gestiona360.Payroll.Application/Features/Companies/Queries/GetAllBranchesQueryHandler.cs

using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Application.Mappers;
using Gestiona360.Payroll.Domain.Interfaces;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.Companies.Queries;

public class GetAllBranchesQueryHandler : IRequestHandler<GetAllBranchesQuery, List<BranchDto>>
{
    private readonly IBranchRepository _branchRepository;

    public GetAllBranchesQueryHandler(IBranchRepository branchRepository)
    {
        _branchRepository = branchRepository ?? throw new ArgumentNullException(nameof(branchRepository));
    }

    public async Task<List<BranchDto>> Handle(GetAllBranchesQuery request, CancellationToken cancellationToken)
    {
        var branches = await _branchRepository.GetAllWithDetailsAsync(cancellationToken);

        return branches.Select(BranchMapper.ToDto).ToList();
    }
}
