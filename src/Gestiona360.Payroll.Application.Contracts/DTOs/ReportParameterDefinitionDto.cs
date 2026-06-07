using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.DTOs
{
    public record ReportParameterDefinitionDto(
        string Name,
        string Type,          // "int", "string", "date", "guid"
        string Label,
        bool IsRequired,
        string? DefaultValue
    );
}
