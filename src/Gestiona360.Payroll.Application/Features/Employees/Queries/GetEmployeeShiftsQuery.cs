using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Application.Contracts.DTOs.Employees;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.Employees.Queries
{
    public record GetEmployeeShiftsQuery(Guid EmployeeId) : IRequest<EmployeeShiftsResultDto>;
}
