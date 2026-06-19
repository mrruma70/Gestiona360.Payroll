using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.Config.Queries;

public class GetGlobalConfigQuery : IRequest<GlobalConfigDto>
{
    public int? YearINSS { get; }
    public int? YearIR { get; }
    public int? YearMitrab { get; }
    public int? YearINATEC { get; }

    public GetGlobalConfigQuery(int? yearINSS, int? yearIR, int? yearMitrab, int? yearINATEC)
    {
        YearINSS = yearINSS;
        YearIR = yearIR;
        YearMitrab = yearMitrab;
        YearINATEC = yearINATEC;
    }
}