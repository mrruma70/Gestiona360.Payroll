using Gestiona360.Payroll.API.Contracts.Common;
using Gestiona360.Payroll.Application.Contracts.DTOs;
using MediatR;

namespace Gestiona360.Payroll.Application.Queries
{
    public record GetPayrollFrequenciesPagedQuery(
        int Page,
        int PageSize,
        string? Sort,
        string? Dir
    ) : IRequest<PagedResult<PayrollFrequencyDto>>;
}
