using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Domain.Shared.Frontend;

namespace Gestiona360.Payroll.Application.Mappers
{
    public static class BranchMapper
    {
        /// <summary>
        /// Mapea BranchWithDetailsInfo a BranchDto.
        /// </summary>
        public static BranchDto ToDto(BranchWithDetailsInfo b)
        {
            return new BranchDto
            {
                Id = b.Id,
                Code = b.Code,
                Name = b.Name,
                Address = b.Address,
                City = b.City,
                Phone = b.Phone,
                CompanyId = b.CompanyId,
                IsActive = b.IsActive,
                IsZoneFranca = b.IsZoneFranca,
                ManagerEmployeeId = b.ManagerEmployeeId,
                DefaultCostCenterId = b.DefaultCostCenterId,
                // ✅ Concatenar nombre completo del manager
                ManagerEmployeeName = b.ManagerFirstName != null && b.ManagerLastName != null
                    ? $"{b.ManagerFirstName} {b.ManagerLastName}"
                    : null,
                CostCenterName = b.CostCenterName
            };
        }
    }
}
