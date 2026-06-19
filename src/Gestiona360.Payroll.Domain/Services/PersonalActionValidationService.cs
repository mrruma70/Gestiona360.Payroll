using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Interfaces;
using Gestiona360.Payroll.Domain.Shared.Frontend;

namespace Gestiona360.Payroll.Domain.Services
{
    public class PersonalActionValidationService : IPersonalActionValidationService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IJobGradeRepository _jobGradeRepository;
        private readonly IMinimumWageRepository _minimumWageRepository;
        private readonly IEmployeeSuspensionRepository _suspensionRepository;
        private readonly IEmployeeShiftAssignmentRepository _shiftAssignmentRepository;

        public PersonalActionValidationService(
            IEmployeeRepository employeeRepository,
            IJobGradeRepository jobGradeRepository,
            IMinimumWageRepository minimumWageRepository,
            IEmployeeSuspensionRepository suspensionRepository,
            IEmployeeShiftAssignmentRepository shiftAssignmentRepository)
        {
            _employeeRepository = employeeRepository;
            _jobGradeRepository = jobGradeRepository;
            _minimumWageRepository = minimumWageRepository;
            _suspensionRepository = suspensionRepository;
            _shiftAssignmentRepository = shiftAssignmentRepository;
        }

        public async Task<bool> IsSalaryAboveMinimumAsync(Guid employeeId, decimal newSalary, CancellationToken ct)
        {
            var employee = await _employeeRepository.GetByIdWithJobDetailsAsync(employeeId, ct);
            if (employee?.JobGrade?.JobPosition == null)
                return true;

            var minimumWageId = employee.JobGrade.JobPosition.MinimumWageId;
            if (!minimumWageId.HasValue)
                return true;

            var minimumWage = await _minimumWageRepository.GetByIdAsync(minimumWageId.Value, ct);
            if (minimumWage == null || !minimumWage.IsActive)
                return true;

            return newSalary >= minimumWage.MonthlyAmountNIO;
        }

        public async Task<bool> IsNewJobGradeDifferentAsync(Guid employeeId, Guid? newJobGradeId, CancellationToken ct)
        {
            if (!newJobGradeId.HasValue)
                return false;

            var employee = await _employeeRepository.GetByIdAsync(employeeId, ct);
            if (employee == null)
                return false;

            if (!employee.JobGradeId.HasValue)
                return true;

            return newJobGradeId.Value != employee.JobGradeId.Value;
        }

        public async Task<bool> IsNewJobGradeSalaryValidAsync(Guid employeeId, Guid? newJobGradeId, CancellationToken ct)
        {
            if (!newJobGradeId.HasValue)
                return true;

            var employee = await _employeeRepository.GetByIdWithJobDetailsAsync(employeeId, ct);
            if (employee == null)
                return true;

            var newJobGrade = await _jobGradeRepository.GetByIdWithPositionAsync(newJobGradeId.Value, ct);
            if (newJobGrade?.JobPosition?.MinimumWage == null)
                return true;

            var minimumWage = newJobGrade.JobPosition.MinimumWage.MonthlyAmountNIO;
            return employee.BaseSalary >= minimumWage;
        }

        public async Task<bool> IsNewContractTypeDifferentAsync(Guid employeeId, int? newContractTypeId, CancellationToken ct)
        {
            if (!newContractTypeId.HasValue)
                return false;

            var employee = await _employeeRepository.GetByIdAsync(employeeId, ct);
            if (employee == null)
                return false;

            if (!employee.ContractTypeId.HasValue)
                return true;

            return newContractTypeId.Value != employee.ContractTypeId.Value;
        }

        public async Task<bool> IsNewShiftDifferentAsync(Guid employeeId, Guid? newShiftId, CancellationToken ct)
        {
            if (!newShiftId.HasValue)
                return false;

            var activeShift = await _shiftAssignmentRepository.GetActiveShiftByEmployeeIdAsync(employeeId, ct);
            if (activeShift == null)
                return true;

            return newShiftId.Value != activeShift.ShiftId;
        }

        public async Task<bool> IsNewCostCenterDifferentAsync(Guid employeeId, Guid? newCostCenterId, CancellationToken ct)
        {
            if (!newCostCenterId.HasValue)
                return false;

            var employee = await _employeeRepository.GetByIdAsync(employeeId, ct);
            if (employee == null)
                return false;

            if (!employee.CostCenterId.HasValue)
                return true;

            return newCostCenterId.Value != employee.CostCenterId.Value;
        }

        public async Task<bool> IsNewHealthProviderDifferentAsync(Guid employeeId, Guid? newHealthProviderId, CancellationToken ct)
        {
            if (!newHealthProviderId.HasValue)
                return false;

            var employee = await _employeeRepository.GetByIdAsync(employeeId, ct);
            if (employee == null)
                return false;

            if (!employee.HealthProviderId.HasValue)
                return true;

            return newHealthProviderId.Value != employee.HealthProviderId.Value;
        }

        public async Task<bool> IsNewBankAccountDifferentAsync(Guid employeeId, int? newBankId, string? newAccountNumber, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(newAccountNumber))
                return false;

            var employee = await _employeeRepository.GetByIdAsync(employeeId, ct);
            if (employee == null)
                return false;

            if (!employee.BankId.HasValue || string.IsNullOrWhiteSpace(employee.BankAccountNumber))
                return true;

            return newBankId != employee.BankId ||
                   newAccountNumber.Trim() != employee.BankAccountNumber.Trim();
        }

        public async Task<bool> HasNoActiveSuspensionAsync(Guid employeeId, CancellationToken ct)
        {
            var activeSuspension = await _suspensionRepository.GetActiveSuspensionByEmployeeIdAsync(employeeId, ct);
            return activeSuspension == null;
        }

        public async Task<bool> IsEmployeeSuspendedAsync(Guid employeeId, CancellationToken ct)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId, ct);
            return employee != null && employee.EmploymentStatus == EmploymentStatus.Suspended;
        }

        public async Task<bool> CanBeTerminatedAsync(Guid employeeId, CancellationToken ct)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId, ct);
            return employee != null &&
                   (employee.EmploymentStatus == EmploymentStatus.Active ||
                    employee.EmploymentStatus == EmploymentStatus.Suspended);
        }

        //public async Task<bool> IsNewHealthProviderDifferentAsync(Guid employeeId, Guid? newHealthProviderId, CancellationToken ct)
        //{
        //    if (!newHealthProviderId.HasValue)
        //        return false;

        //    var employee = await _employeeRepository.GetByIdAsync(employeeId, ct);
        //    if (employee == null)
        //        return false;

        //    // Si el empleado no tiene proveedor de salud actual, cualquier valor es válido
        //    if (!employee.HealthProviderId.HasValue)
        //        return true;

        //    // Debe ser diferente al actual
        //    return newHealthProviderId.Value != employee.HealthProviderId.Value;
        //}
    }
}
