using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Application.Features.Employees.Reports;
using Gestiona360.Payroll.Application.Services;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace Gestiona360.Payroll.Application.Features.Employees.Queries
{
    /// <summary>
    /// Handler para generar el PDF de credencial tipo carnet.
    /// </summary>
    public class GenerateEmployeeCredentialQueryHandler : IRequestHandler<GenerateEmployeeCredentialQuery, byte[]>
    {
        private readonly ApplicationDbContext _context;
        private readonly EmployeeBarcodeService _barcodeService;

        public GenerateEmployeeCredentialQueryHandler(
            ApplicationDbContext context,
            EmployeeBarcodeService barcodeService)
        {
            _context = context;
            _barcodeService = barcodeService;
        }

        public async Task<byte[]> Handle(GenerateEmployeeCredentialQuery request, CancellationToken cancellationToken)
        {
            var employee = await _context.Employees
                .Include(e => e.Company)
                .Include(e => e.Branch)
                .Include(e => e.JobGrade)
                    .ThenInclude(jg => jg!.JobPosition)
                .Include(e => e.PayrollGroup)
                .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken);

            if (employee == null)
                throw new KeyNotFoundException("Empleado no encontrado.");

            var document = new EmployeeCredentialDocument(employee, request.WebRootPath, _barcodeService);
            return document.GeneratePdf();
        }
    }
}
