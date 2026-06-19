using Gestiona360.Payroll.Domain.Services;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Commands
{

    public class GetMassActionPreviewQueryHandler
        : IRequestHandler<GetMassActionPreviewQuery, MassActionPreviewDto>
    {
        private readonly MassActionPreviewDomainService _domainService;

        public GetMassActionPreviewQueryHandler(MassActionPreviewDomainService domainService)
        {
            _domainService = domainService ?? throw new ArgumentNullException(nameof(domainService));
        }

        public async Task<MassActionPreviewDto> Handle(
            GetMassActionPreviewQuery request,
            CancellationToken cancellationToken)
        {
            return await _domainService.GeneratePreviewAsync(request.Data, cancellationToken);
        }
    }
}
