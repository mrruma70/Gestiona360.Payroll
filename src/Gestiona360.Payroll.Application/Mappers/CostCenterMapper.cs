using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Domain.Shared.Frontend;

namespace Gestiona360.Payroll.Application.Mappers
{
    public static class CostCenterMapper
    {
        public static CostCenterDto ToDto(CostCenterBasicInfo c)
        {
            return new CostCenterDto
            {
                Id = c.Id,
                Code = c.Code,
                Name = c.Name,
                CostType = c.CostType,
                IsActive = c.IsActive
            };
        }
    }
}
