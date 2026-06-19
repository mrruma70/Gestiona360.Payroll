using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Interfaces;
using Gestiona360.Payroll.Domain.Shared.Frontend;

namespace Gestiona360.Payroll.Application.Services
{
    public class EmployeeExportService : IEmployeeExportService
    {
        private readonly IEmployeeExportRepository _exportRepository;
        private readonly IExcelGenerator _excelGenerator;

        public EmployeeExportService(
            IEmployeeExportRepository exportRepository,
            IExcelGenerator excelGenerator)
        {
            _exportRepository = exportRepository ?? throw new ArgumentNullException(nameof(exportRepository));
            _excelGenerator = excelGenerator ?? throw new ArgumentNullException(nameof(excelGenerator));
        }

        public async Task<byte[]> ExportToExcelAsync(EmployeeExportFilters filters, CancellationToken ct = default)
        {
            // 1. Obtener datos filtrados del repositorio
            var employees = await _exportRepository.GetEmployeesForExportAsync(filters, ct);

            // 2. Generar el archivo Excel
            return _excelGenerator.GenerateEmployeeExport(employees);
        }
    }
}
