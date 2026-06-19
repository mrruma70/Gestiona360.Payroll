using Gestiona360.Payroll.Application.Contracts.DTOs.Employees;
using Gestiona360.Payroll.Application.Mappers;
using Gestiona360.Payroll.Infrastructure.Persistence;
using Gestiona360.Payroll.Infrastructure.Persistence.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.Employees.Queries
{

    public class GetEmployeePayrollHistoryQueryHandler : IRequestHandler<GetEmployeePayrollHistoryQuery, EmployeePayrollHistoryResultDto>
    {
        private readonly IPayrollRepository _payrollRepository;

        public GetEmployeePayrollHistoryQueryHandler(IPayrollRepository payrollRepository)
        {
            _payrollRepository = payrollRepository ?? throw new ArgumentNullException(nameof(payrollRepository));
        }

        public async Task<EmployeePayrollHistoryResultDto> Handle(GetEmployeePayrollHistoryQuery request, CancellationToken cancellationToken)
        {
            var data = await _payrollRepository.GetPayrollHistoryAsync(request.EmployeeId, cancellationToken);

            if (data == null)
                throw new KeyNotFoundException($"Empleado con ID {request.EmployeeId} no encontrado.");

            return PayrollMapper.ToHistoryResultDto(data);
        }
    }
}
