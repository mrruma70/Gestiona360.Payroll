

using Gestiona360.Payroll.Domain.Shared.Frontend;

namespace Gestiona360.Payroll.Domain.Interfaces
{
    public interface IGlobalConfigRepository
    {
        Task<GlobalConfigDto> GetGlobalConfigAsync(
            int? yearINSS,
            int? yearIR,
            int? yearMitrab,
            int? yearINATEC,
            CancellationToken ct = default);
    }
}
