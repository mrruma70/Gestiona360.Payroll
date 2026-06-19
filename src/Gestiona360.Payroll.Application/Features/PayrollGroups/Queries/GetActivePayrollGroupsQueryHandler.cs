using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Application.Mappers;
using Gestiona360.Payroll.Domain.Interfaces;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.PayrollGroups.Queries
{
    public class GetActivePayrollGroupsQueryHandler
        : IRequestHandler<GetActivePayrollGroupsQuery, List<PayrollGroupDto>>
    {
        private readonly IPayrollGroupRepository _payrollGroupRepository;

        public GetActivePayrollGroupsQueryHandler(IPayrollGroupRepository payrollGroupRepository)
        {
            _payrollGroupRepository = payrollGroupRepository
                ?? throw new ArgumentNullException(nameof(payrollGroupRepository));
        }

        public async Task<List<PayrollGroupDto>> Handle(
            GetActivePayrollGroupsQuery request,
            CancellationToken cancellationToken)
        {
            var payrollGroups = await _payrollGroupRepository
                .GetActivePayrollGroupsAsync(cancellationToken);

            return payrollGroups.Select(PayrollGroupMapper.ToDto).ToList();
        }
    }
}