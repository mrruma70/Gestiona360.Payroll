// src/Gestiona360.Payroll.Application/Services/EmployeeFilterService.cs

using System.Linq.Expressions;
using Gestiona360.Payroll.Application.Features.Employees.Queries;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Shared.Frontend;

namespace Gestiona360.Payroll.Application.Services;

public static class EmployeeFilterService
{
    public static Expression<Func<Employee, bool>>? BuildPredicate(GetEmployeesQuery request)
    {
        Expression<Func<Employee, bool>>? predicate = null;

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.ToLower();
            predicate = And(predicate, e =>
                e.FirstName.ToLower().Contains(searchTerm) ||
                e.LastName.ToLower().Contains(searchTerm) ||
                e.Identification.ToLower().Contains(searchTerm) ||
                e.Code.ToLower().Contains(searchTerm) ||
                e.Email.ToLower().Contains(searchTerm) ||
                e.NORUC.ToLower().Contains(searchTerm) ||
                e.NOINSS.ToLower().Contains(searchTerm));
        }

        if (request.BranchId.HasValue)
            predicate = And(predicate, e => e.BranchId == request.BranchId.Value);

        if (request.ContractTypeId.HasValue)
            predicate = And(predicate, e => e.ContractTypeId == request.ContractTypeId.Value);

        if (!string.IsNullOrEmpty(request.Status))
        {
            var isActive = request.Status.ToLower() == "active";
            predicate = And(predicate, e => e.IsActive == isActive);
        }

        if (request.EmploymentStatus.HasValue)
            predicate = And(predicate, e => e.EmploymentStatus == (EmploymentStatus)request.EmploymentStatus.Value);

        if (request.JobPositionId.HasValue)
            predicate = And(predicate, e => e.JobGrade != null && e.JobGrade.JobPositionId == request.JobPositionId.Value);

        if (request.IsTrustEmployee.HasValue)
            predicate = And(predicate, e => e.IsTrustEmployee == request.IsTrustEmployee.Value);

        if (request.IsForeignWorker.HasValue)
        {
            predicate = request.IsForeignWorker.Value
                ? And(predicate, e => !string.IsNullOrEmpty(e.Nationality))
                : And(predicate, e => string.IsNullOrEmpty(e.Nationality));
        }

        if (request.IsOnProbation.HasValue)
        {
            var now = DateTime.UtcNow;
            predicate = request.IsOnProbation.Value
                ? And(predicate, e => e.ProbationStartDate.HasValue &&
                                     e.ProbationEndDate.HasValue &&
                                     now >= e.ProbationStartDate.Value &&
                                     now <= e.ProbationEndDate.Value)
                : And(predicate, e => !e.ProbationStartDate.HasValue ||
                                     !e.ProbationEndDate.HasValue ||
                                     now < e.ProbationStartDate.Value ||
                                     now > e.ProbationEndDate.Value);
        }

        if (request.IsRehire.HasValue)
        {
            predicate = request.IsRehire.Value
                ? And(predicate, e => e.PreviousEmployeeId.HasValue)
                : And(predicate, e => !e.PreviousEmployeeId.HasValue);
        }

        if (request.PayrollGroupId.HasValue)
            predicate = And(predicate, e => e.PayrollGroupId == request.PayrollGroupId.Value);

        return predicate;
    }

    private static Expression<Func<T, bool>>? And<T>(
        Expression<Func<T, bool>>? left,
        Expression<Func<T, bool>> right)
    {
        if (left == null) return right;

        var parameter = Expression.Parameter(typeof(T), "e");
        var body = Expression.AndAlso(
            Expression.Invoke(left, parameter),
            Expression.Invoke(right, parameter));

        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }
}