using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs
{
    public record ReportDefinitionDto(
      string Code,
      string Name,
      string Description,
      string DefaultFormat,
      IReadOnlyList<ReportParameterDefinitionDto> Parameters
  );
}
