using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Exceptions;
using Gestiona360.Payroll.Domain.Interfaces;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.Config.Queries
{

    public class GetEmployeeBarcodeInfoQueryHandler
        : IRequestHandler<GetEmployeeBarcodeInfoQuery, EmployeeBarcodeInfo>
    {
        private readonly IEmployeeRepository _repository;

        public GetEmployeeBarcodeInfoQueryHandler(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public async Task<EmployeeBarcodeInfo> Handle(GetEmployeeBarcodeInfoQuery request, CancellationToken cancellationToken)
        {
            var employee = await _repository.GetByIdAsync(request.EmployeeId, cancellationToken)
                ?? throw new NotFoundException("Empleado", request.EmployeeId);

            return new EmployeeBarcodeInfo
            {
                Code = employee.Code,
                CompanyId = employee.CompanyId,
                ExistingBarcode = employee.CodigoBarra
            };
        }
    }
}
