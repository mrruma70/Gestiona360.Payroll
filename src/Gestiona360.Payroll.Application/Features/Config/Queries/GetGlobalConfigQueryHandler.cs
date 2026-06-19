// src/Gestiona360.Payroll.Application/Features/Config/Queries/GetGlobalConfigQueryHandler.cs

using Gestiona360.Payroll.Domain.Interfaces;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.Config.Queries;

public class GetGlobalConfigQueryHandler : IRequestHandler<GetGlobalConfigQuery, GlobalConfigDto>
{
    private readonly IGlobalConfigRepository _repository;

    public GetGlobalConfigQueryHandler(IGlobalConfigRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<GlobalConfigDto> Handle(GetGlobalConfigQuery request, CancellationToken cancellationToken)
    {
        // Normalizar 0 a null (igual que el código original)
        int? yearINSS = request.YearINSS == 0 ? null : request.YearINSS;
        int? yearIR = request.YearIR == 0 ? null : request.YearIR;
        int? yearMitrab = request.YearMitrab == 0 ? null : request.YearMitrab;
        int? yearINATEC = request.YearINATEC == 0 ? null : request.YearINATEC;

        return await _repository.GetGlobalConfigAsync(
            yearINSS, yearIR, yearMitrab, yearINATEC, cancellationToken);
    }
}