using Gestiona360.Payroll.Application.Features.Employees.Reports;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace Gestiona360.Payroll.Application.Features.Employees.Queries;

public class GenerateEmployeePdfQueryHandler : IRequestHandler<GenerateEmployeePdfQuery, byte[]>
{
    private readonly ApplicationDbContext _context;

    public GenerateEmployeePdfQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<byte[]> Handle(GenerateEmployeePdfQuery request, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees
            .Include(e => e.Company)
            .Include(e => e.Branch)
            .Include(e => e.CostCenter)
            .Include(e => e.ContractType)
            .Include(e => e.JobGrade).ThenInclude(jg => jg!.JobPosition)
            .Include(e => e.Bank)
            .Include(e => e.HealthProvider)
            .Include(e => e.OccupationalRisk)
             .Include(e => e.PayrollGroup)
              .Include(e => e.Department)         
                .Include(e => e.Municipality)     
            .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken);

        if (employee == null)
            throw new KeyNotFoundException("Empleado no encontrado.");

        // ✅ Aquí usa request.WebRootPath
        var document = new EmployeeFichaDocument(employee, request.WebRootPath);
        return document.GeneratePdf();
    }
}