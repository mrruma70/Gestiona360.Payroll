using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Infrastructure.Reporting.Renderers;

public class PdfReportRenderer : IReportRenderer
{
    // ✅ Cambiar de SupportedFormat a Format
    public string Format => "Pdf";
    public string MimeType => "application/pdf";
    public string FileExtension => "pdf";

    public PdfReportRenderer()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    // ✅ Cambiar el parámetro de IEnumerable<IDictionary<string, object>> a IEnumerable<dynamic>
    public Task<byte[]> RenderAsync(string reportName, IEnumerable<dynamic> data, CancellationToken ct = default)
    {
        // Convertir dynamic a IDictionary<string, object> para procesamiento interno
        var dataList = data.Select(d => (IDictionary<string, object>)d).ToList();
        var headers = dataList.Any() ? dataList[0].Keys.ToList() : new List<string>();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(1.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(9));

                // HEADER
                page.Header().Column(headerColumn =>
                {
                    headerColumn.Item().Row(row =>
                    {
                        row.RelativeItem().Text(reportName).FontSize(16).Bold().FontColor(Colors.Blue.Darken2);
                        row.RelativeItem().AlignRight().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(10).FontColor(Colors.Grey.Medium);
                    });
                    headerColumn.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                });

                // CONTENT
                page.Content().PaddingVertical(10).Element(contentContainer =>
                {
                    if (!dataList.Any())
                    {
                        contentContainer.AlignCenter().AlignMiddle().Text("No hay datos para mostrar")
                            .FontSize(12).FontColor(Colors.Grey.Medium);
                        return;
                    }

                    contentContainer.Table(table =>
                    {
                        // Definir columnas
                        table.ColumnsDefinition(columns =>
                        {
                            foreach (var _ in headers)
                            {
                                columns.RelativeColumn();
                            }
                        });

                        // Encabezado de la tabla
                        table.Header(header =>
                        {
                            foreach (var h in headers)
                            {
                                header.Cell()
                                    .Background(Colors.Blue.Lighten4)
                                    .Padding(4)
                                    .Text(h)
                                    .Bold();
                            }
                        });

                        // Filas de datos
                        foreach (var row in dataList)
                        {
                            foreach (var h in headers)
                            {
                                var value = row.TryGetValue(h, out var val) ? FormatValue(val) : "";

                                table.Cell()
                                    .BorderBottom(1)
                                    .BorderColor(Colors.Grey.Lighten3)
                                    .Padding(3)
                                    .Text(value);
                            }
                        }
                    });
                });

                // FOOTER
                page.Footer().AlignCenter().Text(txt =>
                {
                    txt.Span("Página ");
                    txt.CurrentPageNumber();
                    txt.Span(" de ");
                    txt.TotalPages();
                });
            });
        });

        return Task.FromResult(document.GeneratePdf());
    }

    private static string FormatValue(object? value) => value switch
    {
        null => "",
        DateTime dt => dt.ToString("dd/MM/yyyy"),
        decimal d => d.ToString("N2"),
        double d => d.ToString("N2"),
        float f => f.ToString("N2"),
        bool b => b ? "Sí" : "No",
        _ => value.ToString() ?? ""
    };
}