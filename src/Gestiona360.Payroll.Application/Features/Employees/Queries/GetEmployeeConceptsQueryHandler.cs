using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Application.Contracts.DTOs.Employees;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.Employees.Queries
{
    public class GetEmployeeConceptsQueryHandler : IRequestHandler<GetEmployeeConceptsQuery, EmployeeConceptsResultDto>
    {
        private readonly ApplicationDbContext _context;

        public GetEmployeeConceptsQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EmployeeConceptsResultDto> Handle(GetEmployeeConceptsQuery request, CancellationToken cancellationToken)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken);

            if (employee == null)
                throw new KeyNotFoundException("Empleado no encontrado.");

            // 1. Obtener conceptos recurrentes (Préstamos, bonos, deducciones contractuales)
            var concepts = await _context.EmployeeConceptSettings
                .Include(ecs => ecs.PayrollConcept)
                .Where(ecs => ecs.EmployeeId == request.EmployeeId)
                .OrderByDescending(ecs => ecs.StartDate)
                .Select(ecs => new EmployeeConceptLineDto
                {
                    Id = ecs.Id,
                    ConceptName = ecs.PayrollConcept.Name,
                    Type = ecs.PayrollConcept.Type,
                    Category = ecs.PayrollConcept.Category,
                    IsActive = ecs.IsActive,
                    Amount = ecs.CustomValue > 0 ? (decimal)ecs.CustomValue : (decimal)ecs.InstallmentAmount,
                    Periodicity = ecs.ApplicationPeriodicity,
                    InstallmentsInfo = (ecs.IsRecurrent && ecs.InstallmentTotal > 0)
                        ? $"{ecs.InstallmentCurrent} de {ecs.InstallmentTotal}"
                        : null,
                    RemainingBalance = ecs.IsRecurrent ? ecs.RemainingBalance : null,
                    StartDate = ecs.StartDate,
                    EndDate = ecs.EndDate
                })
                .ToListAsync(cancellationToken);

            // 2. Obtener retenciones judiciales (Embargos / Pensión Alimenticia)
            var garnishments = await _context.Garnishments
                .Where(g => g.EmployeeId == request.EmployeeId)
                .OrderByDescending(g => g.StartDate)
                .Select(g => new EmployeeGarnishmentLineDto
                {
                    Id = g.Id,
                    Type = g.Type,
                    CourtOrderNumber = g.CourtOrderNumber,
                    PercentageLimit = g.PercentageLimit,
                    TotalAmountToWithhold = g.TotalAmountToWithhold ?? 0,
                    WithheldToDate = g.WithheldToDate,
                    IsActive = g.EndDate == null || g.EndDate > DateTime.UtcNow,
                    StartDate = g.StartDate,
                    EndDate = g.EndDate
                })
                .ToListAsync(cancellationToken);

            return new EmployeeConceptsResultDto
            {
                EmployeeName = $"{employee.FirstName} {employee.LastName}",
                EmployeeCode = employee.Code,
                Concepts = concepts,
                Garnishments = garnishments
            };
        }
    }
}
