using System.Dynamic;
using System.Formats.Asn1;
using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;

namespace Gestiona360.Payroll.Infrastructure.Reporting.Renderers;

public class CsvReportRenderer : IReportRenderer
{
    public string Format => "Csv";
    public string MimeType => "text/csv";
    public string FileExtension => ".csv";

    public async Task<byte[]> RenderAsync(string reportName, IEnumerable<dynamic> data, CancellationToken ct = default)
    {
        await using var ms = new MemoryStream();
        await using var writer = new StreamWriter(ms, new UTF8Encoding(true)); // BOM para Excel
        await using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            HasHeaderRecord = true
        });

        var dataList = data.ToList();
        if (dataList.Any())
        {
            // CsvHelper puede escribir diccionarios directamente
            var dictList = dataList.Select(DynamicHelper.ToDictionary).ToList();
            await csv.WriteRecordsAsync(dictList, ct);
        }
        else
        {
            await writer.WriteLineAsync("Sin datos");
        }

        await writer.FlushAsync(ct);
        return ms.ToArray();
    }
}