using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Contracts.Requests
{
    public record ReportResult(
        byte[] Content,
        string FileName,
        string MimeType
    );

}
