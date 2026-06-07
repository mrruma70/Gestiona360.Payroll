using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Infrastructure.Reporting.Renderers;

public class PdfFichaRenderer : IReportRenderer
{
    public string Format => "PdfFicha";
    public string MimeType => "application/pdf";
    public string FileExtension => "pdf";

    public PdfFichaRenderer()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public Task<byte[]> RenderAsync(string reportName, IEnumerable<dynamic> data, CancellationToken ct = default)
    {
        var dataList = data.ToList();
        if (!dataList.Any())
            throw new InvalidOperationException("No hay datos para generar la ficha.");

        // Tomamos el primer registro (es una ficha de UN solo registro)
        var record = (IDictionary<string, object>)dataList[0];

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                // ============================================
                // ENCABEZADO
                // ============================================
                page.Header().Element(header => ComposeHeader(header, reportName));

                // ============================================
                // CONTENIDO (Formulario con secciones)
                // ============================================
                page.Content().PaddingVertical(10).Element(content => ComposeContent(content, record));

                // ============================================
                // PIE DE PÁGINA
                // ============================================
                page.Footer().AlignCenter().Text(txt =>
                {
                    txt.Span("Documento generado el ").FontSize(8).FontColor(Colors.Grey.Medium);
                    txt.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).FontSize(8).FontColor(Colors.Grey.Medium);
                    txt.Span(" | Página ").FontSize(8).FontColor(Colors.Grey.Medium);
                    txt.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
                    txt.Span(" de ").FontSize(8).FontColor(Colors.Grey.Medium);
                    txt.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
                });
            });
        });

        return Task.FromResult(document.GeneratePdf());
    }

    // ============================================
    // ENCABEZADO PROFESIONAL
    // ============================================
    private static void ComposeHeader(IContainer container, string reportName)
    {
        container.Column(col =>
        {
            // Línea superior de color
            col.Item().Height(4).Background(Colors.Blue.Darken2);

            col.Item().PaddingTop(10).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text(reportName.ToUpper())
                        .FontSize(18).Bold().FontColor(Colors.Blue.Darken2);
                    c.Item().Text("FICHA LEGAL DE LA EMPRESA")
                        .FontSize(11).FontColor(Colors.Grey.Medium);
                });

                row.ConstantItem(80).AlignRight().Column(c =>
                {
                    c.Item().AlignRight().Text("Gestiona360")
                        .FontSize(9).Bold().FontColor(Colors.Blue.Darken2);
                    c.Item().AlignRight().Text("Payroll System")
                        .FontSize(8).FontColor(Colors.Grey.Medium);
                });
            });

            // Línea separadora
            col.Item().PaddingTop(5).PaddingBottom(5).LineHorizontal(1).LineColor(Colors.Blue.Darken2);
        });
    }

    // ============================================
    // CONTENIDO: FORMULARIO CON SECCIONES
    // ============================================
    private static void ComposeContent(IContainer container, IDictionary<string, object> record)
    {
        container.Column(col =>
        {
            col.Spacing(8);

            // ============================================
            // SECCIÓN 1: DATOS GENERALES DE LA EMPRESA
            // ============================================
            col.Item().Element(c => ComposeSection(c, "🏢 DATOS GENERALES DE LA EMPRESA", new[]
            {
                ("Nombre Legal", GetStr(record, "Nombre Legal")),
                ("Nombre Comercial", GetStr(record, "Nombre Comercial")),
                ("RUC / NIT", GetStr(record, "RUC/NIT")),
                ("Código CIIU", GetStr(record, "Código CIIU"))
            }));

            // ============================================
            // SECCIÓN 2: DATOS FISCALES Y LABORALES
            // ============================================
            col.Item().Element(c => ComposeSection(c, "📋 DATOS FISCALES Y LABORALES", new[]
            {
                ("Código Patronal INSS", GetStr(record, "Código Patronal INSS")),
                ("Moneda Base", GetStr(record, "Moneda Base")),
                ("Zona Franca", GetStr(record, "Zona Franca")),
                ("Empleados Activos", GetStr(record, "Empleados Activos"))
            }));

            // ============================================
            // SECCIÓN 3: REPRESENTACIÓN LEGAL
            // ============================================
            col.Item().Element(c => ComposeSection(c, "👤 REPRESENTACIÓN LEGAL", new[]
            {
                ("Representante Legal", GetStr(record, "Representante Legal")),
                ("Cédula Rep. Legal", GetStr(record, "Cédula Rep. Legal"))
            }));

            // ============================================
            // SECCIÓN 4: UBICACIÓN
            // ============================================
            col.Item().Element(c => ComposeSection(c, "📍 UBICACIÓN FISCAL", new[]
            {
                ("Dirección Fiscal", GetStr(record, "Dirección Fiscal")),
                ("Ciudad", GetStr(record, "Ciudad")),
                ("Departamento", GetStr(record, "Departamento"))
            }));

            // ============================================
            // SECCIÓN 5: CONTACTO
            // ============================================
            col.Item().Element(c => ComposeSection(c, "📞 DATOS DE CONTACTO", new[]
            {
                ("Teléfono", GetStr(record, "Teléfono")),
                ("Correo Electrónico", GetStr(record, "Correo Electrónico"))
            }));

            // ============================================
            // SECCIÓN 6: INFORMACIÓN DEL SISTEMA
            // ============================================
            col.Item().Element(c => ComposeSection(c, "🕐 INFORMACIÓN DEL SISTEMA", new[]
            {
                ("Fecha Constitución", GetStr(record, "Fecha Creación"))
            }));

            // ============================================
            // FIRMA Y SELLO
            // ============================================
            col.Item().PaddingTop(30).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().PaddingTop(30).LineHorizontal(1).LineColor(Colors.Grey.Darken2);
                    c.Item().AlignCenter().Text("Representante Legal").FontSize(9).FontColor(Colors.Grey.Darken2);
                });

                row.RelativeItem(); // Espacio en blanco

                row.RelativeItem().Column(c =>
                {
                    c.Item().PaddingTop(30).LineHorizontal(1).LineColor(Colors.Grey.Darken2);
                    c.Item().AlignCenter().Text("Sello de la Empresa").FontSize(9).FontColor(Colors.Grey.Darken2);
                });
            });
        });
    }

    // ============================================
    // COMPONENTE REUTILIZABLE: SECCIÓN
    // ============================================
    private static void ComposeSection(IContainer container, string title, IEnumerable<(string Label, string Value)> fields)
    {
        container.Column(col =>
        {
            col.Spacing(4);

            // Título de la sección
            col.Item().Padding(5).Background(Colors.Blue.Lighten4).Text(title)
                .FontSize(11).Bold().FontColor(Colors.Blue.Darken2);

            // Campos en grid de 2 columnas
            col.Item().Grid(grid =>
            {
                grid.Columns(2);
                grid.Spacing(5);

                foreach (var (label, value) in fields)
                {
                    grid.Item(1).Column(c =>
                    {
                        c.Item().Text(label).FontSize(8).FontColor(Colors.Grey.Darken2).Bold();
                        c.Item().PaddingTop(2).Text(string.IsNullOrEmpty(value) ? "—" : value)
                            .FontSize(10).FontColor(Colors.Black);
                        c.Item().PaddingTop(2).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
                    });
                }
            });
        });
    }

    // ============================================
    // HELPER: Obtener valor del diccionario
    // ============================================
    private static string GetStr(IDictionary<string, object> record, string key)
    {
        if (record.TryGetValue(key, out var value) && value != null)
        {
            if (value is DateTime dt) return dt.ToString("dd/MM/yyyy");
            if (value is bool b) return b ? "Sí" : "No";
            return value.ToString() ?? "";
        }
        return "";
    }
}