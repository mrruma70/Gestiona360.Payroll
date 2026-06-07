using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Infrastructure.Reporting.Renderers
{
    public interface IReportRenderer
    {
        string Format { get; }              // "Excel", "Csv", "Pdf", "Xml"
        string MimeType { get; }
        string FileExtension { get; }       // ".xlsx", ".csv", ".pdf", ".xml"
        Task<byte[]> RenderAsync(
            string reportName,
            IEnumerable<dynamic> data,
            CancellationToken ct = default);
    }
}
