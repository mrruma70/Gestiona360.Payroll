using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Interfaces;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.Config.Queries
{
    public class GetEmployeeMinimumWageQueryHandler
        : IRequestHandler<GetEmployeeMinimumWageQuery, decimal>
    {
        private readonly IEmployeeRepository _repository;

        public GetEmployeeMinimumWageQueryHandler(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public async Task<decimal> Handle(GetEmployeeMinimumWageQuery request, CancellationToken cancellationToken)
        {
            var minWage = await _repository.GetMinimumWageForEmployeeAsync(request.EmployeeId, cancellationToken);
            return minWage ?? 0m;
        }
    }
}
