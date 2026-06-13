using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Shared.Frontend;

namespace Gestiona360.Payroll.Application.Features.Employees.Reports
{
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
                page.Margin(1.5f, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(9));

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Página ");
                    x.CurrentPageNumber();
                    x.Span(" de ");
                    x.TotalPages();
                });
            });
        }

        // ═══════════════════════════════════════════════════════════════
        // ENCABEZADO
        // ═══════════════════════════════════════════════════════════════

        private void ComposeHeader(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().Row(row =>
                {
                    row.ConstantItem(80).Height(80).Element(ComposeEmployeePhoto);
                    row.ConstantItem(10);

                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text(_employee.Company?.LegalName ?? "EMPRESA").FontSize(14).Bold();
                        c.Item().Text($"RUC: {_employee.Company?.TaxId ?? "N/A"} | INSS: {_employee.Company?.INSSEmployerCode ?? "N/A"}").FontSize(8).FontColor(Colors.Grey.Darken1);
                        c.Item().PaddingTop(3).Text(GetFullName()).FontSize(12).Bold();
                        c.Item().Text($"{_employee.JobGrade?.JobPosition?.Name ?? "Sin puesto"} - {_employee.JobGrade?.Name ?? ""}").FontSize(9).FontColor(Colors.Grey.Darken1);
                    });

                    row.ConstantItem(130).AlignRight().Column(c =>
                    {
                        c.Item().Text("FICHA DEL EMPLEADO").FontSize(11).Bold().FontColor(Colors.Blue.Darken2);
                        c.Item().PaddingTop(3).Element(ComposeStatusBadge);
                        c.Item().PaddingTop(2).Text($"Código: {_employee.Code}").FontSize(8).FontColor(Colors.Grey.Darken1);
                    });
                });

                col.Item().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
            });
        }

        private void ComposeStatusBadge(IContainer container)
        {
            var (statusText, statusColor) = GetEmploymentStatusInfo();
            container
                .Background(statusColor)
                .Padding(3)
                .AlignCenter()
                .Text(statusText)
                .FontSize(9)
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

        // ═══════════════════════════════════════════════════════════════
        // CONTENIDO PRINCIPAL
        // ═══════════════════════════════════════════════════════════════

        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(5).Column(col =>
            {
                col.Spacing(8);

                ComposeSectionIdentification(col);
                ComposeSectionContact(col);
                ComposeSectionEmergencyContact(col);
                ComposeSectionFiscalData(col);
                ComposeSectionLaboralData(col);
                ComposeSectionBankData(col);
                ComposeSectionSpecialConditions(col);

                if (!string.IsNullOrEmpty(_employee.Nationality))
                    ComposeSectionForeignWorker(col);

                if (_employee.EmploymentStatus == EmploymentStatus.Suspended)
                    ComposeSectionSuspension(col);

                ComposeSectionDocuments(col);

                if (!string.IsNullOrEmpty(_employee.Notes))
                    ComposeSectionNotes(col);
            });
        }

        // ═══════════════════════════════════════════════════════════════
        // SECCIÓN 1: IDENTIFICACIÓN Y DATOS PERSONALES
        // ═══════════════════════════════════════════════════════════════

        private void ComposeSectionIdentification(ColumnDescriptor col)
        {
            ComposeSectionTitle(col, "📋 IDENTIFICACIÓN Y DATOS PERSONALES");
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                });

                table.Cell().Element(ApplyCellStyle).Text("Nombre Completo:").Bold();
                table.Cell().Element(ApplyCellStyle).Text(GetFullName());
                table.Cell().Element(ApplyCellStyle).Text("Cédula:").Bold();
                table.Cell().Element(ApplyCellStyle).Text(_employee.Identification);

                table.Cell().Element(ApplyCellStyle).Text("Fecha Nacimiento:").Bold();
                table.Cell().Element(ApplyCellStyle).Text(_employee.BirthDate?.ToString("dd/MM/yyyy") ?? "N/A");
                table.Cell().Element(ApplyCellStyle).Text("Edad:").Bold();
                table.Cell().Element(ApplyCellStyle).Text(_employee.BirthDate.HasValue ? $"{CalculateAge(_employee.BirthDate.Value)} años" : "N/A");

                table.Cell().Element(ApplyCellStyle).Text("Sexo:").Bold();
                table.Cell().Element(ApplyCellStyle).Text(GetGenderText(_employee.Gender));
                table.Cell().Element(ApplyCellStyle).Text("Estado Civil:").Bold();
                table.Cell().Element(ApplyCellStyle).Text(GetMaritalStatusText(_employee.MaritalStatus));
            });
        }

        // ═══════════════════════════════════════════════════════════════
        // SECCIÓN 2: CONTACTO Y DOMICILIO
        // ═══════════════════════════════════════════════════════════════

        private void ComposeSectionContact(ColumnDescriptor col)
        {
            ComposeSectionTitle(col, "📞 CONTACTO Y DOMICILIO");
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                });

                table.Cell().Element(ApplyCellStyle).Text("Correo:").Bold();
                table.Cell().Element(ApplyCellStyle).Text(_employee.Email ?? "N/A");
                table.Cell().Element(ApplyCellStyle).Text("Teléfono:").Bold();
                table.Cell().Element(ApplyCellStyle).Text(_employee.Phone ?? "N/A");

                table.Cell().Element(ApplyCellStyle).Text("Celular:").Bold();
                table.Cell().Element(ApplyCellStyle).Text(_employee.MobilePhone ?? "N/A");
                table.Cell().ColumnSpan(2).Element(ApplyCellStyle).Text("");

                table.Cell().Element(ApplyCellStyle).Text("Dirección:").Bold();
                table.Cell().ColumnSpan(3).Element(ApplyCellStyle).Text(_employee.Address ?? "No registrada");

                table.Cell().Element(ApplyCellStyle).Text("Departamento:").Bold();
                table.Cell().Element(ApplyCellStyle).Text(_employee.Department?.Name ?? "N/A");
                table.Cell().Element(ApplyCellStyle).Text("Municipio:").Bold();
                table.Cell().Element(ApplyCellStyle).Text(_employee.Municipality?.Name ?? "N/A");
            });
        }

        // ═══════════════════════════════════════════════════════════════
        // SECCIÓN 3: CONTACTO DE EMERGENCIA
        // ═══════════════════════════════════════════════════════════════

        private void ComposeSectionEmergencyContact(ColumnDescriptor col)
        {
            if (string.IsNullOrEmpty(_employee.EmergencyContactName))
                return;

            ComposeSectionTitle(col, "🚨 CONTACTO DE EMERGENCIA");
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                });

                table.Cell().Element(ApplyCellStyle).Text("Nombre:").Bold();
                table.Cell().Element(ApplyCellStyle).Text(_employee.EmergencyContactName);
                table.Cell().Element(ApplyCellStyle).Text("Teléfono:").Bold();
                table.Cell().Element(ApplyCellStyle).Text(_employee.EmergencyContactPhone ?? "N/A");

                table.Cell().Element(ApplyCellStyle).Text("Parentesco:").Bold();
                table.Cell().ColumnSpan(3).Element(ApplyCellStyle).Text(_employee.EmergencyContactRelationship ?? "N/A");
            });
        }

        // ═══════════════════════════════════════════════════════════════
        // SECCIÓN 4: DATOS FISCALES
        // ═══════════════════════════════════════════════════════════════

        private void ComposeSectionFiscalData(ColumnDescriptor col)
        {
            ComposeSectionTitle(col, "🏛️ DATOS FISCALES");
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                });

                table.Cell().Element(ApplyCellStyle).Text("N° RUC:").Bold();
                table.Cell().Element(ApplyCellStyle).Text(_employee.NORUC ?? "N/A");
                table.Cell().Element(ApplyCellStyle).Text("N° INSS:").Bold();
                table.Cell().Element(ApplyCellStyle).Text(_employee.NOINSS ?? "N/A");
            });
        }

        // ═══════════════════════════════════════════════════════════════
        // SECCIÓN 5: DATOS LABORALES
        // ═══════════════════════════════════════════════════════════════

        private void ComposeSectionLaboralData(ColumnDescriptor col)
        {
            ComposeSectionTitle(col, "💼 DATOS LABORALES");
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                });

                table.Cell().Element(ApplyCellStyle).Text("Puesto / Nivel:").Bold();
                table.Cell().Element(ApplyCellStyle).Text($"{_employee.JobGrade?.JobPosition?.Name ?? "N/A"} - {_employee.JobGrade?.Name ?? "N/A"}");
                table.Cell().Element(ApplyCellStyle).Text("Tipo Contrato:").Bold();
                table.Cell().Element(ApplyCellStyle).Text(_employee.ContractType?.Name ?? "N/A");

                table.Cell().Element(ApplyCellStyle).Text("Sucursal:").Bold();
                table.Cell().Element(ApplyCellStyle).Text(_employee.Branch?.Name ?? "N/A");
                table.Cell().Element(ApplyCellStyle).Text("Centro de Costo:").Bold();
                table.Cell().Element(ApplyCellStyle).Text(_employee.CostCenter?.Name ?? "N/A");

                table.Cell().Element(ApplyCellStyle).Text("Grupo de Nómina:").Bold();
                table.Cell().Element(ApplyCellStyle).Text(_employee.PayrollGroup?.Name ?? "No asignado");
                table.Cell().Element(ApplyCellStyle).Text("Riesgo Ocupacional:").Bold();
                table.Cell().Element(ApplyCellStyle).Text(_employee.OccupationalRisk?.Name ?? "N/A");

                table.Cell().Element(ApplyCellStyle).Text("Fecha Primer Ingreso:").Bold();
                table.Cell().Element(ApplyCellStyle).Text((_employee.FirstHireDate ?? _employee.HireDate).ToString("dd/MM/yyyy"));
                table.Cell().Element(ApplyCellStyle).Text("Fecha Ingreso Actual:").Bold();
                table.Cell().Element(ApplyCellStyle).Text(_employee.HireDate.ToString("dd/MM/yyyy"));

                table.Cell().Element(ApplyCellStyle).Text("Antigüedad:").Bold();
                table.Cell().Element(ApplyCellStyle).Text(GetAntiguedad(_employee.FirstHireDate ?? _employee.HireDate));
                table.Cell().Element(ApplyCellStyle).Text("Proveedor de Salud:").Bold();
                table.Cell().Element(ApplyCellStyle).Text(_employee.HealthProvider?.Name ?? "No asignado");

                table.Cell().Element(ApplyCellStyle).Text("Salario Base:").Bold();
                table.Cell().Element(ApplyCellStyle).Text($"C$ {_employee.BaseSalary:N2}").Bold().FontColor(Colors.Green.Darken2);
                table.Cell().Element(ApplyCellStyle).Text("Usa Reloj Marcas:").Bold();
                table.Cell().Element(ApplyCellStyle).Text(_employee.UsesTimeClock ? "✅ Sí" : "❌ No");
            });
        }

        // ═══════════════════════════════════════════════════════════════
        // SECCIÓN 6: DATOS BANCARIOS
        // ═══════════════════════════════════════════════════════════════

        private void ComposeSectionBankData(ColumnDescriptor col)
        {
            ComposeSectionTitle(col, "🏦 DATOS BANCARIOS");
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                });

                table.Cell().Element(ApplyCellStyle).Text("Banco:").Bold();
                table.Cell().Element(ApplyCellStyle).Text(_employee.Bank?.Name ?? "No asignado");
                table.Cell().Element(ApplyCellStyle).Text("Tipo de Cuenta:").Bold();
                table.Cell().Element(ApplyCellStyle).Text(_employee.BankAccountType ?? "N/A");

                table.Cell().Element(ApplyCellStyle).Text("Número de Cuenta:").Bold();
                table.Cell().Element(ApplyCellStyle).Text(_employee.BankAccountNumber ?? "N/A");
                table.Cell().Element(ApplyCellStyle).Text("Beneficiario:").Bold();
                table.Cell().Element(ApplyCellStyle).Text(_employee.BankBeneficiaryName ?? "N/A");
            });
        }

        // ═══════════════════════════════════════════════════════════════
        // SECCIÓN 7: CONDICIONES ESPECIALES
        // ═══════════════════════════════════════════════════════════════

        private void ComposeSectionSpecialConditions(ColumnDescriptor col)
        {
            ComposeSectionTitle(col, "⚙️ CONDICIONES ESPECIALES");
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                });

                table.Cell().Element(ApplyCellStyle).Text("Empleado de Confianza:").Bold();
                table.Cell().Element(ApplyCellStyle).Text(_employee.IsTrustEmployee ? "✅ Sí (Art. 44)" : "❌ No");
                table.Cell().Element(ApplyCellStyle).Text("Estado Laboral:").Bold();
                table.Cell().Element(ApplyCellStyle).Text(GetEmploymentStatusInfo().text).Bold();

                if (_employee.ProbationStartDate.HasValue)
                {
                    table.Cell().Element(ApplyCellStyle).Text("Período de Prueba:").Bold();
                    var probationText = $"{_employee.ProbationStartDate.Value:dd/MM/yyyy} - {_employee.ProbationEndDate?.ToString("dd/MM/yyyy") ?? "Indefinido"}";
                    if (IsOnProbation()) probationText += " (⏳ EN CURSO)";
                    table.Cell().ColumnSpan(3).Element(ApplyCellStyle).Text(probationText);
                }

                if (_employee.BenefitsInKindValue.HasValue && _employee.BenefitsInKindValue > 0)
                {
                    table.Cell().Element(ApplyCellStyle).Text("Beneficios en Especie:").Bold();
                    table.Cell().ColumnSpan(3).Element(ApplyCellStyle).Text($"C$ {_employee.BenefitsInKindValue:N2} - {_employee.BenefitsInKindDescription ?? "N/A"}");
                }

                if (_employee.PreviousEmployeeId.HasValue)
                {
                    table.Cell().Element(ApplyCellStyle).Text("Reingreso:").Bold();
                    table.Cell().ColumnSpan(3).Element(ApplyCellStyle).Text("✅ Sí (empleado recontratado)");
                }
            });
        }

        // ═══════════════════════════════════════════════════════════════
        // SECCIÓN 8: TRABAJADOR EXTRANJERO
        // ═══════════════════════════════════════════════════════════════

        private void ComposeSectionForeignWorker(ColumnDescriptor col)
        {
            ComposeSectionTitle(col, "🌍 TRABAJADOR EXTRANJERO");
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                });

                table.Cell().Element(ApplyCellStyle).Text("Nacionalidad:").Bold();
                table.Cell().Element(ApplyCellStyle).Text(_employee.Nationality ?? "N/A");
                table.Cell().Element(ApplyCellStyle).Text("N° Permiso Trabajo:").Bold();
                table.Cell().Element(ApplyCellStyle).Text(_employee.WorkPermitNumber ?? "N/A");

                if (_employee.WorkPermitExpirationDate.HasValue)
                {
                    table.Cell().Element(ApplyCellStyle).Text("Vencimiento:").Bold();
                    var expText = _employee.WorkPermitExpirationDate.Value.ToString("dd/MM/yyyy");
                    if (_employee.WorkPermitExpirationDate.Value < DateTime.UtcNow)
                        expText += " (⚠️ VENCIDO)";
                    else if (_employee.WorkPermitExpirationDate.Value < DateTime.UtcNow.AddMonths(3))
                        expText += " (⚠️ Por vencer)";
                    table.Cell().ColumnSpan(3).Element(ApplyCellStyle).Text(expText);
                }
            });
        }

        // ═══════════════════════════════════════════════════════════════
        // SECCIÓN 9: SUSPENSIÓN
        // ═══════════════════════════════════════════════════════════════

        private void ComposeSectionSuspension(ColumnDescriptor col)
        {
            ComposeSectionTitle(col, "⏸️ INFORMACIÓN DE SUSPENSIÓN", Colors.Orange.Darken2);
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                });

                if (_employee.SuspensionStartDate.HasValue)
                {
                    table.Cell().Element(ApplyCellStyle).Text("Fecha Inicio:").Bold();
                    table.Cell().Element(ApplyCellStyle).Text(_employee.SuspensionStartDate.Value.ToString("dd/MM/yyyy"));
                }
                if (_employee.SuspensionEndDate.HasValue)
                {
                    table.Cell().Element(ApplyCellStyle).Text("Fecha Fin:").Bold();
                    table.Cell().Element(ApplyCellStyle).Text(_employee.SuspensionEndDate.Value.ToString("dd/MM/yyyy"));
                }
                if (!string.IsNullOrEmpty(_employee.MitrabAuthorizationNumber))
                {
                    table.Cell().Element(ApplyCellStyle).Text("Autorización MITRAB:").Bold();
                    table.Cell().Element(ApplyCellStyle).Text(_employee.MitrabAuthorizationNumber);
                }
                if (!string.IsNullOrEmpty(_employee.SuspensionJustification))
                {
                    table.Cell().Element(ApplyCellStyle).Text("Justificación:").Bold();
                    table.Cell().ColumnSpan(3).Element(ApplyCellStyle).Text(_employee.SuspensionJustification);
                }
            });
        }

        // ═══════════════════════════════════════════════════════════════
        // SECCIÓN 10: DOCUMENTOS DE IDENTIDAD
        // ═══════════════════════════════════════════════════════════════

        private void ComposeSectionDocuments(ColumnDescriptor col)
        {
            ComposeSectionTitle(col, "🪪 DOCUMENTOS DE IDENTIDAD");
            col.Item().Row(row =>
            {
                row.RelativeItem().PaddingRight(5).Element(c => ComposeIdImage(c, "Cédula Frontal", _employee.IdFrontUrl));
                row.RelativeItem().PaddingLeft(5).Element(c => ComposeIdImage(c, "Cédula Trasera", _employee.IdBackUrl));
            });
        }

        // ═══════════════════════════════════════════════════════════════
        // SECCIÓN 11: NOTAS
        // ═══════════════════════════════════════════════════════════════

        private void ComposeSectionNotes(ColumnDescriptor col)
        {
            ComposeSectionTitle(col, "📝 NOTAS Y OBSERVACIONES");
            col.Item().Padding(5).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(_employee.Notes).FontSize(8);
        }

        // ═══════════════════════════════════════════════════════════════
        // MÉTODOS AUXILIARES DE ESTILO (CORREGIDOS)
        // ═══════════════════════════════════════════════════════════════

        private void ComposeSectionTitle(ColumnDescriptor col, string title, string? color = null)
        {
            col.Item().PaddingTop(5).Text(title).FontSize(11).Bold()
                .FontColor(color != null ? color : Colors.Blue.Darken2);
            col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
        }

        private void ComposeIdImage(IContainer container, string title, string? imageUrl)
        {
            container.Column(col =>
            {
                col.Item().Text(title).FontSize(9).Bold().AlignCenter();
                col.Item().PaddingTop(3).Height(120).Border(1).BorderColor(Colors.Grey.Lighten2).Element(c =>
                {
                    var imagePath = GetPhysicalPath(imageUrl);
                    if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                    {
                        c.Image(imagePath, ImageScaling.FitArea);
                    }
                    else
                    {
                        c.AlignCenter().AlignMiddle().Text("Sin imagen").FontColor(Colors.Grey.Lighten1).FontSize(9);
                    }
                });
            });
        }

        // ✅ CORREGIDO: Método de estilo sin recursión
        private static IContainer ApplyCellStyle(IContainer container)
        {
            return container
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(3);
        }

        // ═══════════════════════════════════════════════════════════════
        // MÉTODOS HELPER
        // ═══════════════════════════════════════════════════════════════

        private string GetFullName()
        {
            var parts = new[] { _employee.FirstName, _employee.SecondName, _employee.LastName, _employee.SecondLastName }
                .Where(p => !string.IsNullOrWhiteSpace(p));
            return string.Join(" ", parts);
        }

        private string GetGenderText(string? gender) => gender switch
        {
            "M" => "Masculino",
            "F" => "Femenino",
            _ => "No especificado"
        };

        private string GetMaritalStatusText(string? status) => status switch
        {
            "S" => "Soltero/a",
            "C" => "Casado/a",
            "U" => "Unión libre",
            "D" => "Divorciado/a",
            "V" => "Viudo/a",
            _ => "No especificado"
        };

        private int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }

        private string GetAntiguedad(DateTime hireDate)
        {
            var now = DateTime.Now;
            var years = now.Year - hireDate.Year;
            var months = now.Month - hireDate.Month;
            if (months < 0) { years--; months += 12; }
            if (years > 0) return $"{years} año{(years > 1 ? "s" : "")}, {months} mes{(months != 1 ? "es" : "")}";
            if (months > 0) return $"{months} mes{(months > 1 ? "es" : "")}";
            return "Recién ingresado";
        }

        private bool IsOnProbation()
        {
            if (!_employee.ProbationStartDate.HasValue) return false;
            var now = DateTime.UtcNow;
            return now >= _employee.ProbationStartDate.Value &&
                   (!_employee.ProbationEndDate.HasValue || now <= _employee.ProbationEndDate.Value);
        }

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