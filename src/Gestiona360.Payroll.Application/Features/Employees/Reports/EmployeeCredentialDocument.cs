using Gestiona360.Payroll.Application.Services;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Gestiona360.Payroll.Application.Features.Employees.Reports
{
    /// <summary>
    /// Documento PDF de credencial tipo gafete/carné profesional.
    /// Tamaño estándar: 10cm x 7cm (tamaño tarjeta de presentación grande).
    /// </summary>
    public class EmployeeCredentialDocument : IDocument
    {
        private readonly Employee _employee;
        private readonly string _webRootPath;
        private readonly EmployeeBarcodeService _barcodeService;

        public EmployeeCredentialDocument(Employee employee, string webRootPath, EmployeeBarcodeService barcodeService)
        {
            _employee = employee;
            _webRootPath = webRootPath;
            _barcodeService = barcodeService;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            // Tamaño tipo gafete/carné: 10cm x 7cm (283 x 198 points)
            container.Page(page =>
            {
                page.Size(new PageSize(283, 198, Unit.Point));
                page.Margin(0);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(7));

                page.Content().Element(ComposeCredential);
            });
        }

        private void ComposeCredential(IContainer container)
        {
            container
                .Width(283)
                .Height(198)
                .Border(2)
                .BorderColor(Colors.Blue.Darken2)
                .Background(Colors.White)
                .Column(col =>
                {
                    // ═══════════════════════════════════════════════════════════════
                    // ENCABEZADO: Franja superior con nombre de empresa
                    // ═══════════════════════════════════════════════════════════════
                    col.Item().Height(35).Element(ComposeHeader);

                    // ═══════════════════════════════════════════════════════════════
                    // CUERPO: Foto + Datos + QR
                    // ═══════════════════════════════════════════════════════════════
                    col.Item().Height(140).Padding(8).Element(ComposeBody);

                    // ═══════════════════════════════════════════════════════════════
                    // PIE: Código y fecha
                    // ═══════════════════════════════════════════════════════════════
                    col.Item().Height(23).Element(ComposeFooter);
                });
        }

        // ═══════════════════════════════════════════════════════════════
        // ENCABEZADO
        // ═══════════════════════════════════════════════════════════════

        private void ComposeHeader(IContainer container)
        {
            container
                .Background(Colors.Blue.Darken2)
                .PaddingHorizontal(10)
                .PaddingVertical(5)
                .Column(col =>
                {
                    col.Item().AlignCenter().Text("DISTRIBUIDORA COMERCIAL DE NICARAGUA, S.A.")
                        .FontSize(11).Bold().FontColor(Colors.White);

                    col.Item().AlignCenter().Text("CREDENCIAL DE EMPLEADO")
                        .FontSize(8).FontColor(Colors.Blue.Lighten3);
                });
        }

        // ═══════════════════════════════════════════════════════════════
        // CUERPO PRINCIPAL
        // ═══════════════════════════════════════════════════════════════

        private void ComposeBody(IContainer container)
        {
            container.Row(row =>
            {
                // ── LADO IZQUIERDO: Foto + Datos personales (65%) ──
                row.RelativeItem(65).Column(leftCol =>
                {
                    leftCol.Spacing(3);

                    // Fila 1: Foto + Nombre
                    leftCol.Item().Row(photoRow =>
                    {
                        // Foto del empleado
                        photoRow.ConstantItem(55).Height(65).Element(ComposePhoto);
                        photoRow.ConstantItem(5);

                        // Nombre y puesto
                        photoRow.RelativeItem().Column(dataCol =>
                        {
                            dataCol.Item().Text($"{_employee.FirstName} {_employee.SecondName}".Trim())
                                .FontSize(10).Bold().FontColor(Colors.Blue.Darken2);

                            dataCol.Item().Text($"{_employee.LastName} {_employee.SecondLastName}".Trim())
                                .FontSize(10).Bold().FontColor(Colors.Blue.Darken2);

                            dataCol.Item().PaddingTop(3).Text(_employee.JobGrade?.JobPosition?.Name ?? "Sin puesto")
                                .FontSize(8).Bold();

                            dataCol.Item().Text(_employee.JobGrade?.Name ?? "")
                                .FontSize(7).FontColor(Colors.Grey.Darken1);
                        });
                    });

                    // Línea divisoria
                    leftCol.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);

                    // Datos de identificación
                    leftCol.Item().Row(r =>
                    {
                        r.ConstantItem(50).Text("CÉDULA:").Bold().FontSize(6).FontColor(Colors.Grey.Darken2);
                        r.RelativeItem().Text(_employee.Identification).FontSize(7);
                    });

                    leftCol.Item().Row(r =>
                    {
                        r.ConstantItem(50).Text("INSS:").Bold().FontSize(6).FontColor(Colors.Grey.Darken2);
                        r.RelativeItem().Text(_employee.NOINSS ?? "N/A").FontSize(7);
                    });

                    leftCol.Item().Row(r =>
                    {
                        r.ConstantItem(50).Text("SUCURSAL:").Bold().FontSize(6).FontColor(Colors.Grey.Darken2);
                        r.RelativeItem().Text(_employee.Branch?.Name ?? "N/A").FontSize(7);
                    });

                    leftCol.Item().Row(r =>
                    {
                        r.ConstantItem(50).Text("GRUPO:").Bold().FontSize(6).FontColor(Colors.Grey.Darken2);
                        r.RelativeItem().Text(_employee.PayrollGroup?.Name ?? "N/A").FontSize(7);
                    });
                });

                row.ConstantItem(5);

                // ── LADO DERECHO: QR + Estado (35%) ──
                row.RelativeItem(35).Column(rightCol =>
                {
                    rightCol.Spacing(2);

                    // Código QR
                    rightCol.Item().AlignCenter().Height(85).Element(ComposeQrCode);

                    // Código del empleado
                    rightCol.Item().AlignCenter().Text(_employee.Code)
                        .FontSize(10).Bold().FontColor(Colors.Blue.Darken2);

                    // Badge de estado
                    rightCol.Item().PaddingTop(2).Element(ComposeStatusBadge);
                });
            });
        }

        // ═══════════════════════════════════════════════════════════════
        // PIE DE PÁGINA
        // ═══════════════════════════════════════════════════════════════

        private void ComposeFooter(IContainer container)
        {
            container
                .Background(Colors.Grey.Lighten4)
                .BorderTop(0.5f)
                .BorderColor(Colors.Grey.Lighten2)
                .PaddingHorizontal(8)
                .PaddingVertical(3)
                .Row(row =>
                {
                    row.RelativeItem().Text($"Emitido: {DateTime.Now:dd/MM/yyyy HH:mm}")
                        .FontSize(5).FontColor(Colors.Grey.Darken1);

                    row.RelativeItem().AlignCenter()
                        .Text("Este documento es propiedad de la empresa")
                        .FontSize(5).FontColor(Colors.Grey.Darken1).Italic();

                    row.RelativeItem().AlignRight()
                        .Text("Si encuentra esta credencial, favor devolverla")
                        .FontSize(5).FontColor(Colors.Grey.Darken1);
                });
        }

        // ═══════════════════════════════════════════════════════════════
        // COMPONENTES VISUALES
        // ═══════════════════════════════════════════════════════════════

        private void ComposePhoto(IContainer container)
        {
            var photoPath = GetPhysicalPath(_employee.PhotoUrl);
            if (!string.IsNullOrEmpty(photoPath) && File.Exists(photoPath))
            {
                container
                    .Border(1)
                    .BorderColor(Colors.Blue.Darken2)
                    .Image(photoPath, ImageScaling.FitArea);
            }
            else
            {
                container
                    .Background(Colors.Blue.Lighten3)
                    .Border(1)
                    .BorderColor(Colors.Blue.Darken2)
                    .AlignCenter()
                    .AlignMiddle()
                    .Text($"{GetInitial(_employee.FirstName)}{GetInitial(_employee.LastName)}")
                    .FontSize(20)
                    .FontColor(Colors.White)
                    .Bold();
            }
        }

        private void ComposeQrCode(IContainer container)
        {
            try
            {
                var payload = _barcodeService.GeneratePayload(_employee.Code, _employee.CompanyId);
                var qrBytes = _barcodeService.GenerateQrCode(payload, pixelsPerModule: 5);

                // Guardar temporalmente para QuestPDF
                var tempPath = Path.Combine(Path.GetTempPath(), $"qr_cred_{_employee.Id}_{Guid.NewGuid():N}.png");
                File.WriteAllBytes(tempPath, qrBytes);

                container.Image(tempPath, ImageScaling.FitArea);

                // Programar eliminación del archivo temporal
                Task.Delay(2000).ContinueWith(_ =>
                {
                    try { File.Delete(tempPath); } catch { }
                });
            }
            catch
            {
                container
                    .Background(Colors.Grey.Lighten3)
                    .Border(1)
                    .BorderColor(Colors.Grey.Lighten2)
                    .AlignCenter()
                    .AlignMiddle()
                    .Text("QR")
                    .FontSize(10)
                    .FontColor(Colors.Grey.Darken1);
            }
        }

        private void ComposeStatusBadge(IContainer container)
        {
            var (statusText, statusColor) = GetEmploymentStatusInfo();
            container
                .Background(statusColor)
                .PaddingVertical(3)
                .PaddingHorizontal(6)
                .AlignCenter()
                .Text(statusText)
                .FontSize(7)
                .FontColor(Colors.White)
                .Bold();
        }

        private (string text, string color) GetEmploymentStatusInfo()
        {
            return _employee.EmploymentStatus switch
            {
                EmploymentStatus.Active => ("ACTIVO", Colors.Green.Darken2),
                EmploymentStatus.Suspended => ("SUSPENDIDO", Colors.Orange.Darken2),
                EmploymentStatus.Terminated => ("TERMINADO", Colors.Red.Darken2),
                _ => ("DESCONOCIDO", Colors.Grey.Darken2)
            };
        }

        // ═══════════════════════════════════════════════════════════════
        // MÉTODOS AUXILIARES
        // ═══════════════════════════════════════════════════════════════

        private string? GetPhysicalPath(string? relativeUrl)
        {
            if (string.IsNullOrEmpty(relativeUrl)) return null;
            var cleanPath = relativeUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
            return Path.Combine(_webRootPath, cleanPath);
        }

        private string GetInitial(string? name)
        {
            if (string.IsNullOrEmpty(name)) return "";
            return name.Substring(0, 1).ToUpper();
        }
    }
}