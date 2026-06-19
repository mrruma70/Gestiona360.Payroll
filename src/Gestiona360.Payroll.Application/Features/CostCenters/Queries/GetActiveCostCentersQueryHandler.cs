using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Application.Mappers;
using Gestiona360.Payroll.Domain.Interfaces;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.CostCenters.Queries
{

    public class GetActiveCostCentersQueryHandler : IRequestHandler<GetActiveCostCentersQuery, List<CostCenterDto>>
    {
        private readonly ICostCenterRepository _costCenterRepository;

        public GetActiveCostCentersQueryHandler(ICostCenterRepository costCenterRepository)
        {
            _costCenterRepository = costCenterRepository ?? throw new ArgumentNullException(nameof(costCenterRepository));
        }

        public async Task<List<CostCenterDto>> Handle(GetActiveCostCentersQuery request, CancellationToken cancellationToken)
        {
            var costCenters = await _costCenterRepository.GetActiveCostCentersAsync(cancellationToken);

            return costCenters.Select(CostCenterMapper.ToDto).ToList();
        }
    }
}
