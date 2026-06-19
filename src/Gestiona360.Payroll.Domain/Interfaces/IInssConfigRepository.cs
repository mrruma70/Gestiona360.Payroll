using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Entities;

namespace Gestiona360.Payroll.Domain.Interfaces
{
    public interface IInssConfigRepository
    {
        Task<INSSConfig?> GetCurrentActiveConfigAsync(CancellationToken ct = default);
        Task CreateConfigAsync(INSSConfig config, CancellationToken ct = default);
        void UpdateConfig(INSSConfig config);
    }
}
