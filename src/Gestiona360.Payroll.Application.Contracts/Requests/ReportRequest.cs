using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.Requests
{
    public record ReportRequest(
       string ReportCode,           // Ej: "DGI_IR_RETENTION_MONTHLY"
       string Format,               // "Excel" | "Csv" | "Pdf" | "Xml"
       Dictionary<string, object> Parameters  // Ej: { "Year": 2026, "Month": 6 }
   );
}
