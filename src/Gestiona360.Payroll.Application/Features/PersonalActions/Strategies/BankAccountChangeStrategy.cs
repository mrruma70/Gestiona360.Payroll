using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Interfaces;
using Gestiona360.Payroll.Domain.Shared.Frontend.Enums;
using Gestiona360.Payroll.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Strategies
{
    public class BankAccountChangeStrategy : IPersonalActionStrategy
    {
        private readonly ApplicationDbContext _context;

        public BankAccountChangeStrategy(ApplicationDbContext context)
        {
            _context = context;
        }

        public ActionType ActionType => ActionType.BankAccountChange;

        public async Task ExecuteAsync(PersonalAction action, Employee employee, CancellationToken ct)
        {
            // Actualizar Banco
            if (action.NewBankId.HasValue && action.NewBankId.Value > 0)
            {
                var bankExists = await _context.Banks.AnyAsync(b => b.Id == action.NewBankId.Value && b.IsActive, ct);
                if (bankExists)
                    employee.BankId = action.NewBankId.Value;
                else
                    throw new InvalidOperationException($"El banco con ID {action.NewBankId.Value} no existe o está inactivo.");
            }

            // Actualizar Número de Cuenta
            if (!string.IsNullOrEmpty(action.NewBankAccountNumber))
            {
                employee.BankAccountNumber = action.NewBankAccountNumber;
            }

            employee.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);
        }
    }
}
