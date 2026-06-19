using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Entities;

namespace Gestiona360.Payroll.Domain.Interfaces
{
    public interface IEmployeeShiftAssignmentRepository
    {
        Task<EmployeeShiftAssignment?> GetActiveShiftByEmployeeIdAsync(
            Guid employeeId,
            CancellationToken ct = default);
    }
}
