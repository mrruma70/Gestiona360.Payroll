using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.API.Contracts.Common;
using Gestiona360.Payroll.Application.Abstractions.Repositories;
using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Application.Queries;
using MediatR;

namespace Gestiona360.Payroll.Application.Handlers.PayrollFrequency
{
    public class GetPayrollFrequenciesPagedQueryHandler
       : IRequestHandler<GetPayrollFrequenciesPagedQuery, PagedResult<PayrollFrequencyDto>>
    {
        private readonly IPayrollFrequencyRepository _repository;

        public GetPayrollFrequenciesPagedQueryHandler(IPayrollFrequencyRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<PayrollFrequencyDto>> Handle(
            GetPayrollFrequenciesPagedQuery request,
            CancellationToken cancellationToken)
        {
            var (items, total) = await _repository.GetPagedAsync(
                request.Page,
                request.PageSize,
                request.Sort,
                request.Dir,
                cancellationToken);

            return new PagedResult<PayrollFrequencyDto>
            {
                Items = items,
                TotalItems = total
            };
        }
    }
}
