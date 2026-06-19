using Gestiona360.Payroll.Domain.Entities;
namespace Gestiona360.Payroll.Domain.Interfaces;

public interface IJobGradeRepository
{
    Task<JobGrade?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<JobGrade?> GetByIdWithPositionAsync(Guid id, CancellationToken ct);
}
