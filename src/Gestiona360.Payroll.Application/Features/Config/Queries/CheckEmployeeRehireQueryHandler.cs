using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Interfaces;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.Config.Queries
{
    public class CheckEmployeeRehireQueryHandler
        : IRequestHandler<CheckEmployeeRehireQuery, RehireCheckResult>
    {
        private readonly IEmployeeRepository _repository;

        public CheckEmployeeRehireQueryHandler(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        public async Task<RehireCheckResult> Handle(CheckEmployeeRehireQuery request, CancellationToken cancellationToken)
        {
            var existingEmployee = await _repository.GetByIdentificationAsync(request.Identification, cancellationToken);

            if (existingEmployee == null)
            {
                return new RehireCheckResult { IsRehire = false };
            }

            if (existingEmployee.EmploymentStatus == EmploymentStatus.Terminated)
            {
                return new RehireCheckResult
                {
                    IsRehire = true,
                    PreviousEmployeeName = $"{existingEmployee.FirstName} {existingEmployee.LastName}",
                    PreviousEmployeeCode = existingEmployee.Code,
                    TerminationDate = existingEmployee.TerminationDate
                };
            }

            return new RehireCheckResult
            {
                IsRehire = false,
                ErrorMessage = $"Ya existe un empleado activo con la cédula {request.Identification}."
            };
        }
    }
}
