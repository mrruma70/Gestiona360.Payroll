using System.Dynamic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Gestiona360.Payroll.Infrastructure.Reporting.Renderers;

public class XmlReportRenderer : IReportRenderer
{
    public string Format => "Xml";
    public string MimeType => "application/xml";
    public string FileExtension => ".xml";

    public async Task<byte[]> RenderAsync(string reportName, IEnumerable<dynamic> data, CancellationToken ct = default)
    {
        await Task.CompletedTask;

        var dataList = data.ToList();
        var root = new XElement("Report",
            new XAttribute("Name", reportName),
            new XAttribute("GeneratedAt", DateTime.UtcNow.ToString("O")),
            new XAttribute("RecordCount", dataList.Count)
        );

        foreach (var item in dataList)
        {
            var record = new XElement("Record");
            var rowDict = DynamicHelper.ToDictionary(item);

            foreach (var kvp in rowDict)
            {
                var elementName = SanitizeElementName(kvp.Key);
                var value = FormatValue(kvp.Value);
                record.Add(new XElement(elementName, value));
            }
            root.Add(record);
        }

        var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);

        await using var ms = new MemoryStream();
        await using var writer = new StreamWriter(ms, new UTF8Encoding(false));
        doc.Save(writer);
        await writer.FlushAsync(ct);

        return ms.ToArray();
    }

    private static string SanitizeElementName(string name)
    {
        // Reemplaza caracteres no válidos para nombres XML por guiones bajos
        return Regex.Replace(name, @"[^a-zA-Z0-9_]", "_");
    }

    private static string FormatValue(object? value) => value switch
    {
        null => "",
        DateTime dt => dt.ToString("yyyy-MM-dd"),
        decimal d => d.ToString("F2", System.Globalization.CultureInfo.InvariantCulture),
        double d => d.ToString("F2", System.Globalization.CultureInfo.InvariantCulture),
        _ => value.ToString() ?? ""
    };
}