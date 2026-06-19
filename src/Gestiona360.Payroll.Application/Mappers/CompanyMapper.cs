using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Domain.Entities;

namespace Gestiona360.Payroll.Application.Mappers
{
    public static class CompanyMapper
    {
        /// <summary>
        /// Mapea entidad Company a CompanyDto.
        /// </summary>
        public static CompanyDto ToDto(Company c)
        {
            return new CompanyDto
            {
                Id = c.Id,
                LegalName = c.LegalName,
                CommercialName = c.CommercialName,
                TaxId = c.TaxId,
                INSSEmployerCode = c.INSSEmployerCode,
                EconomicActivityCode = c.EconomicActivityCode,
                Phone = c.Phone,
                Email = c.Email,
                Address = c.Address,
                City = c.City,
                Department = c.Department,
                LogoUrl = c.LogoUrl,
                LegalRepresentative = c.LegalRepresentative,
                LegalRepresentativeId = c.LegalRepresentativeId,
                DefaultCurrency = c.DefaultCurrency,
                DefaultPayrollFrequencyId = c.DefaultPayrollFrequencyId,
                DefaultIsZoneFranca = c.DefaultIsZoneFranca
            };
        }
    }
}
