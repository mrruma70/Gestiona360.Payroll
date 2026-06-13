using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.Employees.Queries
{
    /// <summary>
    /// Query para generar el PDF de credencial tipo carnet del empleado.
    /// </summary>
    public class GenerateEmployeeCredentialQuery : IRequest<byte[]>
    {
        public Guid EmployeeId { get; set; }
        public string WebRootPath { get; set; } = string.Empty;

        public GenerateEmployeeCredentialQuery(Guid employeeId, string webRootPath)
        {
            EmployeeId = employeeId;
            WebRootPath = webRootPath;
        }
    }
}
