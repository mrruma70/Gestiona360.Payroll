using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Application.Features.Employees.Commands;
using Gestiona360.Payroll.Domain.Services;
using Gestiona360.Payroll.Domain.Shared.Frontend;

namespace Gestiona360.Payroll.Application.Services
{
    public interface ICsvImportService
    {
        Task<List<DgiTaxBracketCsvRecord>> ReadDgiTaxBracketsAsync(Stream fileStream, CancellationToken ct);
        Task<List<MitrabSalaryCsvRecord>> ReadMitrabSalariesAsync(Stream fileStream, CancellationToken ct);
    }

}
