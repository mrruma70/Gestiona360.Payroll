using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using Gestiona360.Payroll.Application.Contracts.DTOs.Employees;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.Employees.Queries
{
    public class GetEmployeePersonalActionsQueryHandler : IRequestHandler<GetEmployeePersonalActionsQuery, List<PersonalActionDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetEmployeePersonalActionsQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PersonalActionDto>> Handle(GetEmployeePersonalActionsQuery request, CancellationToken cancellationToken)
        {
            var actions = await (
                from pa in _context.PersonalActions
                join u in _context.Users
                    on pa.ApprovedBy equals u.Id into users
                from u in users.DefaultIfEmpty()

                    // JOINs para obtener nombres de las entidades relacionadas
                join oldJobGrade in _context.JobGrades
                    on pa.OldJobGradeId equals oldJobGrade.Id into oldJobGrades
                from oldJobGrade in oldJobGrades.DefaultIfEmpty()

                join newJobGrade in _context.JobGrades
                    on pa.NewJobGradeId equals newJobGrade.Id into newJobGrades
                from newJobGrade in newJobGrades.DefaultIfEmpty()

                join oldContractType in _context.ContractTypes
                    on pa.OldContractTypeId equals oldContractType.Id into oldContractTypes
                from oldContractType in oldContractTypes.DefaultIfEmpty()

                join newContractType in _context.ContractTypes
                    on pa.NewContractTypeId equals newContractType.Id into newContractTypes
                from newContractType in newContractTypes.DefaultIfEmpty()

                join oldShift in _context.Shifts
                    on pa.OldShiftId equals oldShift.Id into oldShifts
                from oldShift in oldShifts.DefaultIfEmpty()

                join newShift in _context.Shifts
                    on pa.NewShiftId equals newShift.Id into newShifts
                from newShift in newShifts.DefaultIfEmpty()

                join oldCostCenter in _context.CostCenters
                    on pa.OldCostCenterId equals oldCostCenter.Id into oldCostCenters
                from oldCostCenter in oldCostCenters.DefaultIfEmpty()

                join newCostCenter in _context.CostCenters
                    on pa.NewCostCenterId equals newCostCenter.Id into newCostCenters
                from newCostCenter in newCostCenters.DefaultIfEmpty()

                where pa.EmployeeId == request.EmployeeId
                orderby pa.EffectiveDate descending
                select new PersonalActionDto
                {
                    Id = pa.Id,
                    EffectiveDate = pa.EffectiveDate,
                    ActionType = pa.ActionType.ToString(),
                    Status = pa.Status.ToString(),
                    Justification = pa.Justification,
                    CausalDescription = pa.CausalDescription,
                    ExecutedDate = pa.ExecutedDate,
                    ApprovedBy = pa.ApprovedBy,
                    ApprovedByName = u != null ? (u.UserName ?? u.Email ?? "Sistema") : "Sistema",

                    // Salario
                    OldBaseSalary = pa.OldBaseSalary,
                    NewBaseSalary = pa.NewBaseSalary,

                    // Puesto
                    OldJobGradeId = pa.OldJobGradeId,
                    OldJobGradeName = oldJobGrade != null ? oldJobGrade.Name : null,
                    NewJobGradeId = pa.NewJobGradeId,
                    NewJobGradeName = newJobGrade != null ? newJobGrade.Name : null,

                    // Contrato
                    OldContractTypeId = pa.OldContractTypeId,
                    OldContractTypeName = oldContractType != null ? oldContractType.Name : null,
                    NewContractTypeId = pa.NewContractTypeId,
                    NewContractTypeName = newContractType != null ? newContractType.Name : null,

                    // Turno
                    OldShiftId = pa.OldShiftId,
                    OldShiftName = oldShift != null ? oldShift.Name : null,
                    NewShiftId = pa.NewShiftId,
                    NewShiftName = newShift != null ? newShift.Name : null,

                    // Centro de Costo
                    OldCostCenterId = pa.OldCostCenterId,
                    OldCostCenterName = oldCostCenter != null ? oldCostCenter.Name : null,
                    NewCostCenterId = pa.NewCostCenterId,
                    NewCostCenterName = newCostCenter != null ? newCostCenter.Name : null,

                    // Compatibilidad con código antiguo
                    OldValue = pa.OldBaseSalary.HasValue ? $"C$ {pa.OldBaseSalary.Value:N2}" : "N/A",
                    NewValue = pa.NewBaseSalary.HasValue ? $"C$ {pa.NewBaseSalary.Value:N2}" : "N/A"
                }
            ).ToListAsync(cancellationToken);

            return actions;
        }
    }
}

