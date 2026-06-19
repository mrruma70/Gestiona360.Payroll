using System;
using System.Collections.Generic;
using System.Text;
using ClosedXML.Excel;
using Gestiona360.Payroll.Domain.Shared.Frontend;

namespace Gestiona360.Payroll.Application.Services
{
    public class ClosedXmlExcelGenerator : IExcelGenerator
    {
        public byte[] GenerateEmployeeExport(IEnumerable<EmployeeExportDto> employees)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Empleados");
            var employeeList = employees.ToList();

            if (!employeeList.Any())
            {
                worksheet.Cell(1, 1).Value = "No hay datos para mostrar";
                return SaveToBytes(workbook);
            }

            WriteHeaders(worksheet);
            WriteData(worksheet, employeeList);
            ApplyFinalFormat(worksheet, employeeList.Count);
            InsertGroupHeaders(worksheet);

            return SaveToBytes(workbook);
        }

        #region Headers

        private static void WriteHeaders(IXLWorksheet worksheet)
        {
            var headers = GetHeaders();
            var groupRanges = GetGroupRanges();
            var groupColors = GetGroupColors();

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.WrapText = true;

                foreach (var (start, end, groupKey) in groupRanges)
                {
                    if (i + 1 >= start && i + 1 <= end)
                    {
                        cell.Style.Fill.BackgroundColor = groupColors[groupKey];
                        break;
                    }
                }
            }
        }

        private static string[] GetHeaders() => new[]
        {
        "Código", "Cédula", "Primer Nombre", "Segundo Nombre",
        "Primer Apellido", "Segundo Apellido", "Fecha Nacimiento", "Sexo", "Estado Civil",
        "Correo", "Teléfono", "Celular", "Dirección", "Departamento", "Municipio",
        "Contacto Emergencia", "Teléfono Emergencia", "Parentesco",
        "RUC", "INSS",
        "Fecha Ingreso", "Fecha Primer Ingreso", "Puesto", "Nivel", "Sucursal", "Código Sucursal",
        "Tipo Contrato", "Salario Base", "Código Centro Costo", "Centro de Costo", "Grupo de Nómina", "Proveedor de Salud",
        "Banco", "Tipo Cuenta", "Cuenta Bancaria", "Beneficiario",
        "Estado Laboral", "Estado Sistema", "Empleado Confianza", "Usa Reloj Marcas", "Inicio Período Prueba", "Fin Período Prueba",
        "Nacionalidad", "Permiso Trabajo", "Vencimiento Permiso",
        "Valor en Especie", "Descripción Especie",
        "Inicio Suspensión", "Fin Suspensión",
        "Es Reingreso",
        "Notas"
    };

        private static (int start, int end, int groupKey)[] GetGroupRanges() => new[]
        {
        (1, 9, 0), (10, 15, 1), (16, 18, 2), (19, 20, 3),
        (21, 32, 4), (33, 36, 5), (37, 42, 6), (43, 45, 7),
        (46, 47, 8), (48, 49, 9), (50, 50, 10), (51, 51, 11)
    };

        private static Dictionary<int, XLColor> GetGroupColors() => new()
    {
        { 0, XLColor.LightSteelBlue }, { 1, XLColor.LightGreen },
        { 2, XLColor.LightPink }, { 3, XLColor.LightYellow },
        { 4, XLColor.LightCyan }, { 5, XLColor.LightBlue },
        { 6, XLColor.Lavender }, { 7, XLColor.Orange },
        { 8, XLColor.LavenderFloral }, { 9, XLColor.Salmon },
        { 10, XLColor.LightGreen }, { 11, XLColor.WhiteSmoke }
    };

        #endregion

        #region Data Writing

        private static void WriteData(IXLWorksheet worksheet, List<EmployeeExportDto> employees)
        {
            for (int rowIdx = 0; rowIdx < employees.Count; rowIdx++)
            {
                var emp = employees[rowIdx];
                var row = rowIdx + 2;
                var col = 1;

                WriteIdentificationGroup(worksheet, row, ref col, emp);
                WriteContactGroup(worksheet, row, ref col, emp);
                WriteEmergencyGroup(worksheet, row, ref col, emp);
                WriteFiscalGroup(worksheet, row, ref col, emp);
                WriteLaboralGroup(worksheet, row, ref col, emp);
                WriteBankGroup(worksheet, row, ref col, emp);
                WriteConditionsGroup(worksheet, row, ref col, emp);
                WriteForeignGroup(worksheet, row, ref col, emp);
                WriteBenefitsGroup(worksheet, row, ref col, emp);
                WriteSuspensionGroup(worksheet, row, ref col, emp);
                WriteRehireGroup(worksheet, row, ref col, emp);
                WriteNotesGroup(worksheet, row, ref col, emp);
            }
        }

        private static void WriteIdentificationGroup(IXLWorksheet ws, int row, ref int col, EmployeeExportDto e)
        {
            ws.Cell(row, col++).Value = e.Codigo;
            ws.Cell(row, col++).Value = e.Cedula;
            ws.Cell(row, col++).Value = e.PrimerNombre ?? "";
            ws.Cell(row, col++).Value = e.SegundoNombre ?? "";
            ws.Cell(row, col++).Value = e.PrimerApellido ?? "";
            ws.Cell(row, col++).Value = e.SegundoApellido ?? "";

            var fechaNacCell = ws.Cell(row, col++);
            if (e.FechaNacimiento.HasValue)
            {
                fechaNacCell.Value = e.FechaNacimiento.Value;
                fechaNacCell.Style.NumberFormat.Format = "yyyy-mm-dd";
            }

            ws.Cell(row, col++).Value = GetGenderText(e.Sexo);
            ws.Cell(row, col++).Value = GetMaritalStatusText(e.EstadoCivil);
        }

        private static void WriteContactGroup(IXLWorksheet ws, int row, ref int col, EmployeeExportDto e)
        {
            ws.Cell(row, col++).Value = e.Correo ?? "";
            ws.Cell(row, col++).Value = e.Telefono ?? "";
            ws.Cell(row, col++).Value = e.Celular ?? "";
            ws.Cell(row, col++).Value = e.Direccion ?? "";
            ws.Cell(row, col++).Value = e.Departamento ?? "";
            ws.Cell(row, col++).Value = e.Municipio ?? "";
        }

        private static void WriteEmergencyGroup(IXLWorksheet ws, int row, ref int col, EmployeeExportDto e)
        {
            ws.Cell(row, col++).Value = e.ContactoEmergencia ?? "";
            ws.Cell(row, col++).Value = e.TelefonoEmergencia ?? "";
            ws.Cell(row, col++).Value = e.Parentesco ?? "";
        }

        private static void WriteFiscalGroup(IXLWorksheet ws, int row, ref int col, EmployeeExportDto e)
        {
            ws.Cell(row, col++).Value = e.RUC ?? "";
            ws.Cell(row, col++).Value = e.INSS ?? "";
        }

        private static void WriteLaboralGroup(IXLWorksheet ws, int row, ref int col, EmployeeExportDto e)
        {
            var fechaIngCell = ws.Cell(row, col++);
            fechaIngCell.Value = e.FechaIngreso;
            fechaIngCell.Style.NumberFormat.Format = "yyyy-mm-dd";

            var fechaPrimIngCell = ws.Cell(row, col++);
            if (e.FechaPrimerIngreso.HasValue)
            {
                fechaPrimIngCell.Value = e.FechaPrimerIngreso.Value;
                fechaPrimIngCell.Style.NumberFormat.Format = "yyyy-mm-dd";
            }

            ws.Cell(row, col++).Value = e.Puesto ?? "Sin puesto";
            ws.Cell(row, col++).Value = e.Nivel ?? "";
            ws.Cell(row, col++).Value = e.Sucursal ?? "";
            ws.Cell(row, col++).Value = e.CodigoSucursal ?? "";
            ws.Cell(row, col++).Value = e.TipoContrato ?? "";

            var salarioCell = ws.Cell(row, col++);
            salarioCell.Value = (double)e.SalarioBase;
            salarioCell.Style.NumberFormat.Format = "#,##0.00";

            ws.Cell(row, col++).Value = e.CostCenterCode ?? "";
            ws.Cell(row, col++).Value = e.CostCenterName ?? "";
            ws.Cell(row, col++).Value = e.GrupoNomina ?? "No asignado";
            ws.Cell(row, col++).Value = e.ProveedorSalud ?? "No asignado";
        }

        private static void WriteBankGroup(IXLWorksheet ws, int row, ref int col, EmployeeExportDto e)
        {
            ws.Cell(row, col++).Value = e.Banco ?? "Sin cuenta";
            ws.Cell(row, col++).Value = e.TipoCuenta ?? "";
            ws.Cell(row, col++).Value = e.CuentaBancaria ?? "";
            ws.Cell(row, col++).Value = e.Beneficiario ?? "";
        }

        private static void WriteConditionsGroup(IXLWorksheet ws, int row, ref int col, EmployeeExportDto e)
        {
            var estadoLaboralCell = ws.Cell(row, col++);
            estadoLaboralCell.Value = GetEmploymentStatusText(e.EmploymentStatus);
            estadoLaboralCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ApplyEmploymentStatusColor(estadoLaboralCell, e.EmploymentStatus);

            var estadoSistemaCell = ws.Cell(row, col++);
            estadoSistemaCell.Value = e.Estado ? "Activo" : "Inactivo";
            estadoSistemaCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            estadoSistemaCell.Style.Font.FontColor = XLColor.White;
            estadoSistemaCell.Style.Fill.BackgroundColor = e.Estado ? XLColor.Green : XLColor.Orange;

            var confianzaCell = ws.Cell(row, col++);
            confianzaCell.Value = e.EsConfianza ? "Sí" : "No";
            confianzaCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            if (e.EsConfianza) confianzaCell.Style.Fill.BackgroundColor = XLColor.LightYellow;

            var relojCell = ws.Cell(row, col++);
            relojCell.Value = e.UsaRelojMarcas ? "Sí" : "No";
            relojCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            if (e.UsaRelojMarcas) relojCell.Style.Fill.BackgroundColor = XLColor.LightCyan;

            var inicioPruebaCell = ws.Cell(row, col++);
            if (e.InicioPrueba.HasValue)
            {
                inicioPruebaCell.Value = e.InicioPrueba.Value;
                inicioPruebaCell.Style.NumberFormat.Format = "yyyy-mm-dd";
            }

            var finPruebaCell = ws.Cell(row, col++);
            if (e.FinPrueba.HasValue)
            {
                finPruebaCell.Value = e.FinPrueba.Value;
                finPruebaCell.Style.NumberFormat.Format = "yyyy-mm-dd";
            }
        }

        private static void WriteForeignGroup(IXLWorksheet ws, int row, ref int col, EmployeeExportDto e)
        {
            ws.Cell(row, col++).Value = e.Nacionalidad ?? "";
            ws.Cell(row, col++).Value = e.PermisoTrabajo ?? "";

            var vencimientoCell = ws.Cell(row, col++);
            if (e.VencimientoPermiso.HasValue)
            {
                vencimientoCell.Value = e.VencimientoPermiso.Value;
                vencimientoCell.Style.NumberFormat.Format = "yyyy-mm-dd";

                if (e.VencimientoPermiso.Value < DateTime.Now)
                {
                    vencimientoCell.Style.Fill.BackgroundColor = XLColor.Red;
                    vencimientoCell.Style.Font.FontColor = XLColor.White;
                }
                else if (e.VencimientoPermiso.Value < DateTime.Now.AddMonths(3))
                {
                    vencimientoCell.Style.Fill.BackgroundColor = XLColor.Orange;
                    vencimientoCell.Style.Font.FontColor = XLColor.White;
                }
            }
        }

        private static void WriteBenefitsGroup(IXLWorksheet ws, int row, ref int col, EmployeeExportDto e)
        {
            var valorEspecieCell = ws.Cell(row, col++);
            if (e.ValorEspecie.HasValue && e.ValorEspecie.Value > 0)
            {
                valorEspecieCell.Value = (double)e.ValorEspecie.Value;
                valorEspecieCell.Style.NumberFormat.Format = "#,##0.00";
            }
            ws.Cell(row, col++).Value = e.DescripcionEspecie ?? "";
        }

        private static void WriteSuspensionGroup(IXLWorksheet ws, int row, ref int col, EmployeeExportDto e)
        {
            var inicioCell = ws.Cell(row, col++);
            if (e.InicioSuspension.HasValue)
            {
                inicioCell.Value = e.InicioSuspension.Value;
                inicioCell.Style.NumberFormat.Format = "yyyy-mm-dd";
            }

            var finCell = ws.Cell(row, col++);
            if (e.FinSuspension.HasValue)
            {
                finCell.Value = e.FinSuspension.Value;
                finCell.Style.NumberFormat.Format = "yyyy-mm-dd";
            }
        }

        private static void WriteRehireGroup(IXLWorksheet ws, int row, ref int col, EmployeeExportDto e)
        {
            var reingresoCell = ws.Cell(row, col++);
            reingresoCell.Value = e.PreviousEmployeeId.HasValue ? "Sí" : "No";
            reingresoCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            if (e.PreviousEmployeeId.HasValue)
                reingresoCell.Style.Fill.BackgroundColor = XLColor.LightGreen;
        }

        private static void WriteNotesGroup(IXLWorksheet ws, int row, ref int col, EmployeeExportDto e)
        {
            ws.Cell(row, col++).Value = e.Notes ?? "";
        }

        #endregion

        #region Final Format

        private static void ApplyFinalFormat(IXLWorksheet worksheet, int rowCount)
        {
            var headers = GetHeaders();
            worksheet.Columns().AdjustToContents();
            worksheet.Range(1, 1, rowCount + 1, headers.Length).SetAutoFilter();
            worksheet.SheetView.FreezeRows(1);
            worksheet.SheetView.FreezeColumns(3);
        }

        private static void InsertGroupHeaders(IXLWorksheet worksheet)
        {
            worksheet.Row(1).InsertRowsAbove(1);

            var groups = new (int startCol, int endCol, string name, XLColor color)[]
            {
            (1, 9, "📋 IDENTIFICACIÓN BÁSICA", XLColor.SteelBlue),
            (10, 15, "📞 CONTACTO Y DOMICILIO", XLColor.Green),
            (16, 18, "🚨 CONTACTO EMERGENCIA", XLColor.Red),
            (19, 20, "🏛️ DATOS FISCALES", XLColor.Gold),
            (21, 32, "💼 DATOS LABORALES", XLColor.DodgerBlue),
            (33, 36, "🏦 DATOS BANCARIOS", XLColor.RoyalBlue),
            (37, 42, "⚙️ CONDICIONES", XLColor.Purple),
            (43, 45, "🌍 EXTRANJERO", XLColor.DarkOrange),
            (46, 47, "💎 BENEFICIOS ESPECIE", XLColor.Olive),
            (48, 49, "⏸️ SUSPENSIÓN", XLColor.Crimson),
            (50, 50, "🔄 REINGRESO", XLColor.SeaGreen),
            (51, 51, "📝 NOTAS", XLColor.Gray)
            };

            foreach (var (startCol, endCol, name, color) in groups)
            {
                var range = worksheet.Range(1, startCol, 1, endCol);
                range.Merge();
                var cell = range.FirstCell();
                cell.Value = name;
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Fill.BackgroundColor = color;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }

            worksheet.Row(1).Height = 25;
        }

        #endregion

        #region Helpers

        private static string GetGenderText(string? gender) => gender switch
        {
            "M" => "Masculino",
            "F" => "Femenino",
            _ => ""
        };

        private static string GetMaritalStatusText(string? status) => status switch
        {
            "S" => "Soltero/a",
            "C" => "Casado/a",
            "U" => "Unión libre",
            "D" => "Divorciado/a",
            "V" => "Viudo/a",
            _ => ""
        };

        private static string GetEmploymentStatusText(int status) => status switch
        {
            (int)EmploymentStatus.Active => "Activo",
            (int)EmploymentStatus.Suspended => "Suspendido",
            (int)EmploymentStatus.Terminated => "Terminado",
            _ => "Desconocido"
        };

        private static void ApplyEmploymentStatusColor(IXLCell cell, int status)
        {
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Fill.BackgroundColor = status switch
            {
                (int)EmploymentStatus.Active => XLColor.Green,
                (int)EmploymentStatus.Suspended => XLColor.Orange,
                (int)EmploymentStatus.Terminated => XLColor.Red,
                _ => XLColor.Gray
            };
        }

        private static byte[] SaveToBytes(XLWorkbook workbook)
        {
            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }

        #endregion
    }
}
