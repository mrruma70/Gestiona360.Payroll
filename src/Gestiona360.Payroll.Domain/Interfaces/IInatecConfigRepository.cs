using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Entities;

namespace Gestiona360.Payroll.Domain.Interfaces
{
    public interface IInatecConfigRepository
    {
        Task<INATECConfig?> GetCurrentActiveConfigAsync(CancellationToken ct = default);
        Task CreateConfigAsync(INATECConfig config, CancellationToken ct = default);
        void UpdateConfig(INATECConfig config);
    }
}
