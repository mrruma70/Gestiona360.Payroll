using Gestiona360.Payroll.Application.Features.PersonalActions.Strategies;
using Gestiona360.Payroll.Domain.Shared.Frontend.Enums;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Commands.ExecutePersonalAction;

public class ExecutePersonalActionCommandHandler : IRequestHandler<ExecutePersonalActionCommand, Unit>
{
    private readonly ApplicationDbContext _context;
    private readonly IEnumerable<IPersonalActionStrategy> _strategies;

    public ExecutePersonalActionCommandHandler(ApplicationDbContext context, IEnumerable<IPersonalActionStrategy> strategies)
    {
        _context = context;
        _strategies = strategies;
    }

    public async Task<Unit> Handle(ExecutePersonalActionCommand request, CancellationToken ct)
    {
        var action = await _context.PersonalActions
            .Include(a => a.Employee)
            .FirstOrDefaultAsync(a => a.Id == request.ActionId, ct);

        // 1. Elegir la estrategia sin switch
        var strategy = _strategies.FirstOrDefault(s => s.ActionType == action.ActionType)
            ?? throw new NotSupportedException($"No hay estrategia para {action.ActionType}");

        // 2. Ejecutar
        await strategy.ExecuteAsync(action, action.Employee, ct);

        action.Status = ActionStatus.Executed;
        await _context.SaveChangesAsync(ct);
        return Unit.Value;
    }

}