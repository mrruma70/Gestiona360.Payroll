using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.Config.Queries
{
    public record GetEmployeeBarcodeInfoQuery(Guid EmployeeId) : IRequest<EmployeeBarcodeInfo>;
}
