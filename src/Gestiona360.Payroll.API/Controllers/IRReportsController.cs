using ClosedXML.Excel;
using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.API.Controllers
{
    [ApiController]
    [Route("api/reports")]
    [Authorize]
    public class IRReportsController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public IRReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("available-years")]
        public async Task<ActionResult<List<int>>> GetAvailableYears()
        {
            // Obtener años desde las tablas de nómina (PayrollPeriods) o desde retenciones
            var years = await _context.PayrollRecords
                .Where(r => r.PayrollPeriod.StartDate.Year >= 2020)
                .Select(r => r.PayrollPeriod.StartDate.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            if (!years.Any())
            {
                years.Add(DateTime.UtcNow.Year);
            }
            return Ok(years);
        }

        [HttpGet("ir-withholding")]
        public async Task<ActionResult<List<WithholdingDto>>> GetIRWithholding(int year, int month)
        {
            // Obtener el período de nómina que corresponde a ese año/mes
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var payrollPeriod = await _context.PayrollPeriods
                .FirstOrDefaultAsync(p => p.StartDate >= startDate && p.EndDate <= endDate);

            if (payrollPeriod == null)
                return Ok(new List<WithholdingDto>());

            // Obtener los registros de nómina del período
            var payrollRecords = await _context.PayrollRecords
                .Include(r => r.Employee)
                .Where(r => r.PayrollPeriodId == payrollPeriod.Id)
                .ToListAsync();

            var result = payrollRecords.Select(r => new WithholdingDto
            {
                EmployeeId = r.Employee.Code ?? r.Employee.Id.ToString(),
                EmployeeName = $"{r.Employee.FirstName} {r.Employee.LastName}",
                GrossSalary = r.GrossIncome,
                INSSWorker = r.INSSWorker,
                TaxableIncome = r.GrossIncome - r.INSSWorker,
                IRWithheld = r.IR
            }).ToList();

            return Ok(result);
        }

        [HttpGet("ir-withholding/excel")]
        public async Task<IActionResult> ExportIRWithholdingExcel(int year, int month)
        {
            var dataResult = await GetIRWithholding(year, month);
            var data = dataResult.Value;
            if (data == null || !data.Any())
                return BadRequest("No hay datos para exportar.");

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add($"IR_{year}_{month:00}");

            // Encabezados
            worksheet.Cell(1, 1).Value = "Año";
            worksheet.Cell(1, 2).Value = "Mes";
            worksheet.Cell(1, 3).Value = "Cédula";
            worksheet.Cell(1, 4).Value = "Empleado";
            worksheet.Cell(1, 5).Value = "Salario Bruto (C$)";
            worksheet.Cell(1, 6).Value = "INSS Laboral (C$)";
            worksheet.Cell(1, 7).Value = "Renta Neta (C$)";
            worksheet.Cell(1, 8).Value = "IR Retenido (C$)";

            int row = 2;
            foreach (var item in data)
            {
                worksheet.Cell(row, 1).Value = year;
                worksheet.Cell(row, 2).Value = month;
                worksheet.Cell(row, 3).Value = item.EmployeeId;
                worksheet.Cell(row, 4).Value = item.EmployeeName;
                worksheet.Cell(row, 5).Value = item.GrossSalary;
                worksheet.Cell(row, 6).Value = item.INSSWorker;
                worksheet.Cell(row, 7).Value = item.TaxableIncome;
                worksheet.Cell(row, 8).Value = item.IRWithheld;
                row++;
            }

            worksheet.Columns().AdjustToContents();
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var bytes = stream.ToArray();
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Declaracion_IR_{year}_{month:00}.xlsx");
        }
    }
}

