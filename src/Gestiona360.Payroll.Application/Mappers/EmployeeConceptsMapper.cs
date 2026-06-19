using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Application.Contracts.DTOs.Employees;
using Gestiona360.Payroll.Domain.Shared.Frontend;

namespace Gestiona360.Payroll.Application.Mappers
{
    public static class EmployeeConceptsMapper
    {
        public static EmployeeConceptsResultDto ToResultDto(EmployeeConceptsData data)
        {
            return new EmployeeConceptsResultDto
            {
                EmployeeName = data.EmployeeName,
                EmployeeCode = data.EmployeeCode,
                Concepts = data.Concepts.Select(ToConceptLineDto).ToList(),
                Garnishments = data.Garnishments.Select(ToGarnishmentLineDto).ToList()
            };
        }

        private static EmployeeConceptLineDto ToConceptLineDto(EmployeeConceptInfo c)
        {
            return new EmployeeConceptLineDto
            {
                Id = c.Id,
                ConceptName = c.ConceptName,
                Type = c.Type,
                Category = c.Category,
                IsActive = c.IsActive,
                Amount = c.Amount,
                Periodicity = c.Periodicity,
                InstallmentsInfo = c.InstallmentsInfo,
                RemainingBalance = c.RemainingBalance,
                StartDate = c.StartDate,
                EndDate = c.EndDate
            };
        }

        private static EmployeeGarnishmentLineDto ToGarnishmentLineDto(GarnishmentInfo g)
        {
            return new EmployeeGarnishmentLineDto
            {
                Id = g.Id,
                Type = g.Type,
                CourtOrderNumber = g.CourtOrderNumber,
                PercentageLimit = g.PercentageLimit,
                TotalAmountToWithhold = g.TotalAmountToWithhold,
                WithheldToDate = g.WithheldToDate,
                IsActive = g.IsActive,
                StartDate = g.StartDate,
                EndDate = g.EndDate
            };
        }
    }
}
