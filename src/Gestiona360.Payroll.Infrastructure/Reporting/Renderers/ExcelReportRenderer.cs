using ClosedXML.Excel;
using System.Data;
using System.Dynamic;

namespace Gestiona360.Payroll.Infrastructure.Reporting.Renderers;

public class ExcelReportRenderer : IReportRenderer
{
    public string Format => "Excel";
    public string MimeType => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    public string FileExtension => "xlsx";

    public async Task<byte[]> RenderAsync(string reportName, IEnumerable<dynamic> data, CancellationToken ct = default)
    {
        await Task.CompletedTask;

        using var workbook = new XLWorkbook();
        var sheetName = reportName.Length > 31 ? reportName[..31] : reportName;
        var worksheet = workbook.Worksheets.Add(sheetName);

        var dataList = data.ToList();

        if (!dataList.Any())
        {
            worksheet.Cell(1, 1).Value = "No hay datos para mostrar";
            return SaveToBytes(workbook);
        }

        // Dapper devuelve objetos que implementan IDictionary<string, object>
        var firstRow = dataList[0] as IDictionary<string, object>;
        if (firstRow == null)
        {
            worksheet.Cell(1, 1).Value = "Error: formato de datos no soportado";
            return SaveToBytes(workbook);
        }

        var headers = firstRow.Keys.ToList();

        // 1. Escribir encabezados
        for (int i = 0; i < headers.Count; i++)
        {
            var cell = worksheet.Cell(1, i + 1);
            cell.Value = FormatHeader(headers[i]);
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.LightSteelBlue;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        // 2. Escribir datos
        for (int rowIdx = 0; rowIdx < dataList.Count; rowIdx++)
        {
            var rowDict = dataList[rowIdx] as IDictionary<string, object>;
            if (rowDict == null) continue;

            for (int colIdx = 0; colIdx < headers.Count; colIdx++)
            {
                var cell = worksheet.Cell(rowIdx + 2, colIdx + 1);
                var value = rowDict.TryGetValue(headers[colIdx], out var val) ? val : null;

                // Manejar diferentes tipos de valores
                if (value == null || value == DBNull.Value)
                {
                    cell.Value = string.Empty;
                }
                else if (value is int intVal)
                {
                    cell.Value = intVal;
                }
                else if (value is long longVal)
                {
                    cell.Value = longVal;
                }
                else if (value is decimal decVal)
                {
                    cell.Value = (double)decVal;
                    cell.Style.NumberFormat.Format = "#,##0.00";
                }
                else if (value is double dblVal)
                {
                    cell.Value = dblVal;
                    cell.Style.NumberFormat.Format = "#,##0.00";
                }
                else if (value is DateTime dateVal)
                {
                    cell.Value = dateVal;
                    cell.Style.NumberFormat.Format = "yyyy-mm-dd";
                }
                else if (value is bool boolVal)
                {
                    cell.Value = boolVal ? "Sí" : "No";
                }
                else
                {
                    cell.Value = value.ToString();
                }
            }
        }

        // 3. Auto-ajustar columnas y aplicar filtros
        worksheet.Columns().AdjustToContents();
        worksheet.Range(1, 1, dataList.Count + 1, headers.Count).SetAutoFilter();
        worksheet.SheetView.FreezeRows(1);

        return SaveToBytes(workbook);
    }

    private static byte[] SaveToBytes(XLWorkbook workbook)
    {
        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
    }

    private static string FormatHeader(string key)
    {
        // Convierte "SalarioBruto" → "Salario Bruto"
        return System.Text.RegularExpressions.Regex.Replace(key, "(\\B[A-Z])", " $1");
    }
}