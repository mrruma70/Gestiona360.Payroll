using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Gestiona360.Payroll.Application.Features.Employees.Reports;

public class EmployeeFichaDocument : IDocument
{
    private readonly Employee _employee;
    private readonly string _webRootPath;

    public EmployeeFichaDocument(Employee employee, string webRootPath)
    {
        _employee = employee;
        _webRootPath = webRootPath;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(2, Unit.Centimetre);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(10));

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().AlignCenter().Text(x =>
            {
                x.Span("Página ");
                x.CurrentPageNumber();
            });
        });
    }

    private void ComposeHeader(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Row(row =>
            {
                // ✅ FOTO DEL EMPLEADO a la izquierda
                row.ConstantItem(80).Height(80).Element(ComposeEmployeePhoto);

                row.ConstantItem(10); // Espacio

                row.RelativeItem().Column(c =>
                {
                    c.Item().Text(_employee.Company?.LegalName ?? "EMPRESA").FontSize(16).Bold();
                    c.Item().Text($"RUC: {_employee.Company?.TaxId ?? "N/A"}").FontSize(10);
                    c.Item().PaddingTop(5).Text($"{_employee.FirstName} {_employee.LastName}").FontSize(12).Bold();
                    c.Item().Text(_employee.JobGrade?.JobPosition?.Name ?? "Sin puesto").FontSize(10).FontColor(Colors.Grey.Darken1);
                });

                // ✅ BADGE DE ESTADO DEL EMPLEADO
                row.ConstantItem(150).AlignRight().Column(c =>
                {
                    c.Item().Text("FICHA DEL EMPLEADO").FontSize(14).Bold().FontColor(Colors.Blue.Darken2);
                    c.Item().PaddingTop(5).Element(ComposeStatusBadge);
                });
            });

            col.Item().PaddingVertical(10).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
        });
    }

    private void ComposeStatusBadge(IContainer container)
    {
        var (statusText, statusColor) = GetEmploymentStatusInfo();

        container
            .Background(statusColor)
            .Padding(5)
            .AlignCenter()
            .Text(statusText)
            .FontSize(10)
            .FontColor(Colors.White)
            .Bold();
    }

    private (string text, string color) GetEmploymentStatusInfo()
    {
        return _employee.EmploymentStatus switch
        {
            EmploymentStatus.Active => ("✅ ACTIVO", Colors.Green.Darken2),
            EmploymentStatus.Suspended => ("⏸️ SUSPENDIDO", Colors.Orange.Darken2),
            EmploymentStatus.Terminated => ("⏹️ TERMINADO", Colors.Red.Darken2),
            _ => ("❓ DESCONOCIDO", Colors.Grey.Darken2)
        };
    }

    private void ComposeEmployeePhoto(IContainer container)
    {
        var photoPath = GetPhysicalPath(_employee.PhotoUrl);

        if (!string.IsNullOrEmpty(photoPath) && File.Exists(photoPath))
        {
            container.Image(photoPath, ImageScaling.FitArea);
        }
        else
        {
            container.Background(Colors.Blue.Lighten3).AlignCenter().AlignMiddle().Text(
                $"{GetInitial(_employee.FirstName)}{GetInitial(_employee.LastName)}"
            ).FontSize(24).FontColor(Colors.White).Bold();
        }
    }

    private void ComposeContent(IContainer container)
    {
        container.PaddingVertical(10).Column(col =>
        {
            col.Spacing(15);

            // Sección 1: Datos Personales
            ComposeSectionPersonalData(col);

            // Sección 2: Datos Fiscales (NUEVO)
            ComposeSectionFiscalData(col);

            // Sección 3: Datos Laborales
            ComposeSectionLaboralData(col);

            // Sección 4: Condiciones Especiales (NUEVO)
            ComposeSectionSpecialConditions(col);

            // Sección 5: Trabajador Extranjero (NUEVO - si aplica)
            if (!string.IsNullOrEmpty(_employee.Nationality))
            {
                ComposeSectionForeignWorker(col);
            }

            // Sección 6: Datos Bancarios
            ComposeSectionBankData(col);

            // Sección 7: Suspensión (NUEVO - si aplica)
            if (_employee.EmploymentStatus == EmploymentStatus.Suspended)
            {
                ComposeSectionSuspension(col);
            }

            // Sección 8: Documentos de Identidad (Cédulas)
            ComposeSectionDocuments(col);

            // Sección 9: Notas (NUEVO - si aplica)
            if (!string.IsNullOrEmpty(_employee.Notes))
            {
                ComposeSectionNotes(col);
            }
        });
    }

    private void ComposeSectionPersonalData(ColumnDescriptor col)
    {
        col.Item().Text("📋 DATOS PERSONALES").FontSize(12).Bold().FontColor(Colors.Blue.Darken2);
        col.Item().Table(table =>
        {
            table.ColumnsDefinition(columns => { columns.RelativeColumn(); columns.RelativeColumn(); });
            table.Cell().Element(CellStyle).Text("Nombre Completo:").Bold();
            table.Cell().Element(CellStyle).Text($"{_employee.FirstName} {_employee.LastName}");
            table.Cell().Element(CellStyle).Text("Cédula:").Bold();
            table.Cell().Element(CellStyle).Text(_employee.Identification);
            table.Cell().Element(CellStyle).Text("Correo:").Bold();
            table.Cell().Element(CellStyle).Text(_employee.Email ?? "N/A");
            table.Cell().Element(CellStyle).Text("Teléfono:").Bold();
            table.Cell().Element(CellStyle).Text(_employee.Phone ?? "N/A");
        });
    }

    private void ComposeSectionFiscalData(ColumnDescriptor col)
    {
        col.Item().PaddingTop(10).Text("🏛️ DATOS FISCALES").FontSize(12).Bold().FontColor(Colors.Blue.Darken2);
        col.Item().Table(table =>
        {
            table.ColumnsDefinition(columns => { columns.RelativeColumn(); columns.RelativeColumn(); });
            table.Cell().Element(CellStyle).Text("N° RUC:").Bold();
            table.Cell().Element(CellStyle).Text(_employee.NORUC ?? "N/A");
            table.Cell().Element(CellStyle).Text("N° INSS:").Bold();
            table.Cell().Element(CellStyle).Text(_employee.NOINSS ?? "N/A");
        });
    }

    private void ComposeSectionLaboralData(ColumnDescriptor col)
    {
        col.Item().PaddingTop(10).Text("🏢 DATOS LABORALES").FontSize(12).Bold().FontColor(Colors.Blue.Darken2);
        col.Item().Table(table =>
        {
            table.ColumnsDefinition(columns => { columns.RelativeColumn(); columns.RelativeColumn(); });
            table.Cell().Element(CellStyle).Text("Puesto / Nivel:").Bold();
            table.Cell().Element(CellStyle).Text($"{_employee.JobGrade?.JobPosition?.Name} - {_employee.JobGrade?.Name}");
            table.Cell().Element(CellStyle).Text("Sucursal:").Bold();
            table.Cell().Element(CellStyle).Text(_employee.Branch?.Name ?? "N/A");

            table.Cell().Element(CellStyle).Text("Centro Costo:").Bold();
            table.Cell().Element(CellStyle).Text(_employee.CostCenter?.Name ?? "N/A");

            table.Cell().Element(CellStyle).Text("Tipo de Contrato:").Bold();
            table.Cell().Element(CellStyle).Text(_employee.ContractType?.Name ?? "N/A");
            table.Cell().Element(CellStyle).Text("Fecha de Ingreso:").Bold();
            table.Cell().Element(CellStyle).Text(_employee.HireDate.ToString("dd/MM/yyyy"));
            table.Cell().Element(CellStyle).Text("Salario Base:").Bold();
            table.Cell().Element(CellStyle).Text($"C$ {_employee.BaseSalary:N2}");
            table.Cell().Element(CellStyle).Text("Riesgo Ocupacional:").Bold();
            table.Cell().Element(CellStyle).Text(_employee.OccupationalRisk?.Name ?? "N/A");
            table.Cell().Element(CellStyle).Text("Proveedor de Salud:").Bold();
            table.Cell().Element(CellStyle).Text(_employee.HealthProvider?.Name ?? "No asignado");
        });
    }

    private void ComposeSectionSpecialConditions(ColumnDescriptor col)
    {
        col.Item().PaddingTop(10).Text("⚙️ CONDICIONES ESPECIALES").FontSize(12).Bold().FontColor(Colors.Blue.Darken2);
        col.Item().Table(table =>
        {
            table.ColumnsDefinition(columns => { columns.RelativeColumn(); columns.RelativeColumn(); });

            table.Cell().Element(CellStyle).Text("Empleado de Confianza:").Bold();
            table.Cell().Element(CellStyle).Text(_employee.IsTrustEmployee ? "✅ Sí (Art. 44)" : "❌ No");

            // Período de Prueba
            if (_employee.ProbationStartDate.HasValue)
            {
                table.Cell().Element(CellStyle).Text("Período de Prueba:").Bold();
                var probationText = $"{_employee.ProbationStartDate.Value:dd/MM/yyyy} - {_employee.ProbationEndDate?.ToString("dd/MM/yyyy") ?? "Indefinido"}";

                if (IsOnProbation())
                    probationText += " (⏳ EN CURSO)";

                table.Cell().Element(CellStyle).Text(probationText);
            }

            // Beneficios en Especie
            if (_employee.BenefitsInKindValue.HasValue && _employee.BenefitsInKindValue > 0)
            {
                table.Cell().Element(CellStyle).Text("Beneficios en Especie:").Bold();
                table.Cell().Element(CellStyle).Text($"C$ {_employee.BenefitsInKindValue:N2} - {_employee.BenefitsInKindDescription ?? "N/A"}");
            }

            // Reingreso
            if (_employee.PreviousEmployeeId.HasValue)
            {
                table.Cell().Element(CellStyle).Text("Reingreso:").Bold();
                table.Cell().Element(CellStyle).Text("✅ Sí (empleado recontratado)");
            }
        });
    }

    private void ComposeSectionForeignWorker(ColumnDescriptor col)
    {
        col.Item().PaddingTop(10).Text("🌍 TRABAJADOR EXTRANJERO").FontSize(12).Bold().FontColor(Colors.Blue.Darken2);
        col.Item().Table(table =>
        {
            table.ColumnsDefinition(columns => { columns.RelativeColumn(); columns.RelativeColumn(); });

            table.Cell().Element(CellStyle).Text("Nacionalidad:").Bold();
            table.Cell().Element(CellStyle).Text(_employee.Nationality ?? "N/A");

            table.Cell().Element(CellStyle).Text("N° Permiso de Trabajo:").Bold();
            table.Cell().Element(CellStyle).Text(_employee.WorkPermitNumber ?? "N/A");

            if (_employee.WorkPermitExpirationDate.HasValue)
            {
                table.Cell().Element(CellStyle).Text("Vencimiento Permiso:").Bold();
                var expText = _employee.WorkPermitExpirationDate.Value.ToString("dd/MM/yyyy");

                if (_employee.WorkPermitExpirationDate.Value < DateTime.UtcNow)
                    expText += " (⚠️ VENCIDO)";
                else if (_employee.WorkPermitExpirationDate.Value < DateTime.UtcNow.AddMonths(3))
                    expText += " (⚠️ Por vencer)";

                table.Cell().Element(CellStyle).Text(expText);
            }
        });
    }

    private void ComposeSectionBankData(ColumnDescriptor col)
    {
        col.Item().PaddingTop(10).Text("🏦 DATOS BANCARIOS").FontSize(12).Bold().FontColor(Colors.Blue.Darken2);
        col.Item().Table(table =>
        {
            table.ColumnsDefinition(columns => { columns.RelativeColumn(); columns.RelativeColumn(); });
            table.Cell().Element(CellStyle).Text("Banco:").Bold();
            table.Cell().Element(CellStyle).Text(_employee.Bank?.Name ?? "No asignado");
            table.Cell().Element(CellStyle).Text("Tipo de Cuenta:").Bold();
            table.Cell().Element(CellStyle).Text(_employee.BankAccountType ?? "N/A");
            table.Cell().Element(CellStyle).Text("Número de Cuenta:").Bold();
            table.Cell().Element(CellStyle).Text(_employee.BankAccountNumber ?? "N/A");
        });
    }

    private void ComposeSectionSuspension(ColumnDescriptor col)
    {
        col.Item().PaddingTop(10).Text("⏸️ INFORMACIÓN DE SUSPENSIÓN").FontSize(12).Bold().FontColor(Colors.Orange.Darken2);
        col.Item().Table(table =>
        {
            table.ColumnsDefinition(columns => { columns.RelativeColumn(); columns.RelativeColumn(); });

            if (_employee.SuspensionStartDate.HasValue)
            {
                table.Cell().Element(CellStyle).Text("Fecha Inicio:").Bold();
                table.Cell().Element(CellStyle).Text(_employee.SuspensionStartDate.Value.ToString("dd/MM/yyyy"));
            }

            if (_employee.SuspensionEndDate.HasValue)
            {
                table.Cell().Element(CellStyle).Text("Fecha Fin:").Bold();
                table.Cell().Element(CellStyle).Text(_employee.SuspensionEndDate.Value.ToString("dd/MM/yyyy"));
            }

            if (!string.IsNullOrEmpty(_employee.MitrabAuthorizationNumber))
            {
                table.Cell().Element(CellStyle).Text("Autorización MITRAB:").Bold();
                table.Cell().Element(CellStyle).Text(_employee.MitrabAuthorizationNumber);
            }

            if (!string.IsNullOrEmpty(_employee.SuspensionJustification))
            {
                table.Cell().Element(CellStyle).Text("Justificación:").Bold();
                table.Cell().Element(CellStyle).Text(_employee.SuspensionJustification);
            }
        });
    }

    private void ComposeSectionDocuments(ColumnDescriptor col)
    {
        col.Item().PaddingTop(10).Text("🪪 DOCUMENTOS DE IDENTIDAD").FontSize(12).Bold().FontColor(Colors.Blue.Darken2);
        col.Item().Row(row =>
        {
            row.RelativeItem().PaddingRight(5).Element(c => ComposeIdImage(c, "Cédula Frontal", _employee.IdFrontUrl));
            row.RelativeItem().PaddingLeft(5).Element(c => ComposeIdImage(c, "Cédula Trasera", _employee.IdBackUrl));
        });
    }

    private void ComposeSectionNotes(ColumnDescriptor col)
    {
        col.Item().PaddingTop(10).Text("📝 NOTAS Y OBSERVACIONES").FontSize(12).Bold().FontColor(Colors.Blue.Darken2);
        col.Item().Padding(5).Border(1).BorderColor(Colors.Grey.Lighten2).Text(_employee.Notes).FontSize(9);
    }

    private void ComposeIdImage(IContainer container, string title, string? imageUrl)
    {
        container.Column(col =>
        {
            col.Item().Text(title).FontSize(10).Bold().AlignCenter();
            col.Item().PaddingTop(5).Height(150).Border(1).BorderColor(Colors.Grey.Lighten2).Element(c =>
            {
                var imagePath = GetPhysicalPath(imageUrl);

                if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                {
                    c.Image(imagePath, ImageScaling.FitArea);
                }
                else
                {
                    c.AlignCenter().AlignMiddle().Text("Sin imagen").FontColor(Colors.Grey.Lighten1).FontSize(10);
                }
            });
        });
    }

    private IContainer CellStyle(IContainer container)
    {
        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(2);
    }

    private bool IsOnProbation()
    {
        if (!_employee.ProbationStartDate.HasValue)
            return false;

        var now = DateTime.UtcNow;
        return now >= _employee.ProbationStartDate.Value &&
               (!_employee.ProbationEndDate.HasValue || now <= _employee.ProbationEndDate.Value);
    }

    private string? GetPhysicalPath(string? relativeUrl)
    {
        if (string.IsNullOrEmpty(relativeUrl))
            return null;

        // Convertir ruta relativa (/uploads/employees/xxx.jpg) a ruta física absoluta
        var cleanPath = relativeUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
        return Path.Combine(_webRootPath, cleanPath);
    }

    private string GetInitial(string? name)
    {
        if (string.IsNullOrEmpty(name))
            return "";
        return name.Substring(0, 1).ToUpper();
    }
}