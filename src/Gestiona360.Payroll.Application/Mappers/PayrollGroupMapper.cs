using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Domain.Shared.Frontend;

namespace Gestiona360.Payroll.Application.Mappers
{
    public static class PayrollGroupMapper
    {
        public static PayrollGroupDto ToDto(PayrollGroupBasicInfo pg)
        {
            return new PayrollGroupDto
            {
                Id = pg.Id,
                Name = pg.Name,
                Code = pg.Code,
                IsActive = pg.IsActive
            };
        }
    }
}
