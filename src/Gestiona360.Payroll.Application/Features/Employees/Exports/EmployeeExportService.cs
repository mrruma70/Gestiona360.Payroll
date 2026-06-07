using ClosedXML.Excel;
using Dapper;

using Gestiona360.Payroll.Domain.Shared.Frontend;
using Gestiona360.Payroll.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using SqlKata;
using SqlKata.Execution;

namespace Gestiona360.Payroll.Application.Features.Employees.Exports
{
    public class EmployeeExportService
    {
        private readonly ApplicationDbContext _context;

        public EmployeeExportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<byte[]> ExportToExcelAsync(
            string? search = null,
            Guid? branchId = null,
            int? contractTypeId = null,
            int? employmentStatus = null,  // ✅ Cambiado de string? a int?
            Guid? jobPositionId = null,
            bool? isTrustEmployee = null,
            bool? isForeignWorker = null,
            bool? isOnProbation = null,    // ✅ NUEVO
            bool? isRehire = null)         // ✅ NUEVO
        {
            // ✅ Construir query con SqlKata (filtros dinámicos)
            var query = new Query("Employees")
     .LeftJoin("JobGrades", "Employees.JobGradeId", "JobGrades.Id")
     .LeftJoin("JobPositions", "JobGrades.JobPositionId", "JobPositions.Id")
     .LeftJoin("Branches", "Employees.BranchId", "Branches.Id")
     // ✅ CORREGIDO: JOIN directo con CostCenters usando Employees.CostCenterId
     .LeftJoin("CostCenters", "Employees.CostCenterId", "CostCenters.Id")
     .LeftJoin("ContractTypes", "Employees.ContractTypeId", "ContractTypes.Id")
    .Select(
    // Datos básicos
    "Employees.Code AS Codigo",
    "Employees.Identification AS Cedula",
    "Employees.FirstName AS Nombres",
    "Employees.LastName AS Apellidos",
    "Employees.Email AS Correo",
    "Employees.Phone AS Telefono",
    "Employees.HireDate AS FechaIngreso",

    // Datos laborales
    "JobPositions.Name AS Puesto",
    "JobGrades.Name AS Nivel",
    "Branches.Name AS Sucursal",
    "Branches.Code AS CodigoSucursal",
    "ContractTypes.Name AS TipoContrato",
    "Employees.BaseSalary AS SalarioBase",

    // ✅ CORREGIDO: Aliases alineados con el DTO
    "CostCenters.Code AS CostCenterCode",
    "CostCenters.Name AS CostCenterName",

    // Datos fiscales
    "Employees.NORUC AS RUC",
    "Employees.NOINSS AS INSS",

    // Estado laboral
    "Employees.EmploymentStatus AS EmploymentStatus",
    "Employees.IsActive AS Estado",

    // Condiciones especiales
    "Employees.IsTrustEmployee AS EsConfianza",
    "Employees.ProbationStartDate AS InicioPrueba",
    "Employees.ProbationEndDate AS FinPrueba",

    // Trabajador extranjero
    "Employees.Nationality AS Nacionalidad",
    "Employees.WorkPermitNumber AS PermisoTrabajo",
    "Employees.WorkPermitExpirationDate AS VencimientoPermiso",

    // Salario en especie
    "Employees.BenefitsInKindValue AS ValorEspecie",
    "Employees.BenefitsInKindDescription AS DescripcionEspecie",

    // Suspensión
    "Employees.SuspensionStartDate AS InicioSuspension",
    "Employees.SuspensionEndDate AS FinSuspension",

    // Reingreso
    "Employees.PreviousEmployeeId AS PreviousEmployeeId",

    // Notas
    "Employees.Notes AS Notas"
);

            // ✅ Aplicar filtros dinámicos
            if (!string.IsNullOrWhiteSpace(search))
            {
                // ✅ Sintaxis correcta para SqlKata con OR
                var searchTerm = $"%{search}%";
                query = query.Where(q => q
                    .OrWhereLike("Employees.FirstName", searchTerm)
                    .OrWhereLike("Employees.LastName", searchTerm)
                    .OrWhereLike("Employees.Identification", searchTerm)
                    .OrWhereLike("Employees.Code", searchTerm)
                    .OrWhereLike("Employees.Email", searchTerm)
                    .OrWhereLike("Employees.NORUC", searchTerm)
                    .OrWhereLike("Employees.NOINSS", searchTerm)
                );
            }

            if (branchId.HasValue)
                query = query.Where("Employees.BranchId", branchId.Value);

            if (contractTypeId.HasValue)
                query = query.Where("Employees.ContractTypeId", contractTypeId.Value);

            // ✅ Filtro por EmploymentStatus (int directo)
            if (employmentStatus.HasValue)
                query = query.Where("Employees.EmploymentStatus", employmentStatus.Value);

            if (jobPositionId.HasValue)
                query = query.Where("JobGrades.JobPositionId", jobPositionId.Value);

            // ✅ Filtro por empleado de confianza
            if (isTrustEmployee.HasValue)
                query = query.Where("Employees.IsTrustEmployee", isTrustEmployee.Value);

            // ✅ Filtro por trabajador extranjero
            if (isForeignWorker.HasValue)
            {
                if (isForeignWorker.Value)
                    query = query.WhereNotNull("Employees.Nationality");
                else
                    query = query.WhereNull("Employees.Nationality");
            }

            // ✅ NUEVO: Filtro por período de prueba
            if (isOnProbation.HasValue)
            {
                var now = DateTime.UtcNow;
                if (isOnProbation.Value)
                {
                    query = query.WhereNotNull("Employees.ProbationStartDate")
                                 .WhereNotNull("Employees.ProbationEndDate")
                                 .Where("Employees.ProbationStartDate", "<=", now)
                                 .Where("Employees.ProbationEndDate", ">=", now);
                }
                else
                {
                    query = query.Where(q => q
                        .OrWhereNull("Employees.ProbationStartDate")
                        .OrWhereNull("Employees.ProbationEndDate")
                        .OrWhere("Employees.ProbationStartDate", ">", now)
                        .OrWhere("Employees.ProbationEndDate", "<", now)
                    );
                }
            }

            // ✅ NUEVO: Filtro por reingreso
            if (isRehire.HasValue)
            {
                if (isRehire.Value)
                    query = query.WhereNotNull("Employees.PreviousEmployeeId");
                else
                    query = query.WhereNull("Employees.PreviousEmployeeId");
            }

            query = query.OrderBy("Employees.FirstName").OrderBy("Employees.LastName");

            // ✅ Ejecutar query con Dapper (SqlKata genera el SQL)
            var connection = _context.Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();

            var compiler = new SqlKata.Compilers.SqlServerCompiler();
            var compiled = compiler.Compile(query);

            var employees = await connection.QueryAsync<EmployeeExportDto>(
                compiled.Sql,
                compiled.NamedBindings
            );

            // ✅ Generar Excel con ClosedXML
            return GenerateExcel(employees.ToList());
        }

        private byte[] GenerateExcel(List<EmployeeExportDto> employees)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Empleados");

            if (!employees.Any())
            {
                worksheet.Cell(1, 1).Value = "No hay datos para mostrar";
                return SaveToBytes(workbook);
            }

            // ✅ Encabezados actualizados con nuevos campos
           var headers = new[]
                  {
                    // Datos básicos (7)
                    "Código", "Cédula", "Nombres", "Apellidos", "Correo", "Teléfono", "Fecha Ingreso",
    
                    // Datos laborales (8) - Centro de costo aquí
                    "Puesto", "Nivel", "Sucursal", "Código Sucursal", "Tipo Contrato", "Salario Base",
                    "Código Centro Costo", "Centro de Costo",
    
                    // Datos fiscales (2)
                    "RUC", "INSS",
    
                    // Estado laboral (2)
                    "Estado Laboral", "Estado Sistema",
    
                    // Condiciones especiales (4)
                    "Empleado Confianza", "Inicio Período Prueba", "Fin Período Prueba", "En Prueba",
    
                    // Trabajador extranjero (3)
                    "Nacionalidad", "Permiso Trabajo", "Vencimiento Permiso",
    
                    // Salario en especie (2)
                    "Valor en Especie", "Descripción Especie",
    
                    // Suspensión (2)
                    "Inicio Suspensión", "Fin Suspensión",
    
                    // Reingreso (1)
                    "Es Reingreso",
    
                    // Notas (1)
                    "Notas"
                };

            // ✅ Estilo de encabezados
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightSteelBlue;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.WrapText = true;
            }

            // ✅ Datos - CORREGIDO: manejo de columnas consistente
            for (int rowIdx = 0; rowIdx < employees.Count; rowIdx++)
            {
                var emp = employees[rowIdx];
                var row = rowIdx + 2;
                var col = 1;

                // Datos básicos (7 columnas)
                worksheet.Cell(row, col++).Value = emp.Codigo;
                worksheet.Cell(row, col++).Value = emp.Cedula;
                worksheet.Cell(row, col++).Value = emp.Nombres;
                worksheet.Cell(row, col++).Value = emp.Apellidos;
                worksheet.Cell(row, col++).Value = emp.Correo;
                worksheet.Cell(row, col++).Value = emp.Telefono;
                var fechaCell = worksheet.Cell(row, col++);
                fechaCell.Value = emp.FechaIngreso;
                fechaCell.Style.NumberFormat.Format = "yyyy-mm-dd";

                // Datos laborales (6 columnas)
                worksheet.Cell(row, col++).Value = emp.Puesto ?? "Sin puesto";
                worksheet.Cell(row, col++).Value = emp.Nivel ?? "";
                worksheet.Cell(row, col++).Value = emp.Sucursal ?? "";
                worksheet.Cell(row, col++).Value = emp.CodigoSucursal ?? "";
                worksheet.Cell(row, col++).Value = emp.TipoContrato ?? "";
                var salarioCell = worksheet.Cell(row, col++);
                salarioCell.Value = (double)emp.SalarioBase;
                salarioCell.Style.NumberFormat.Format = "#,##0.00";

                worksheet.Cell(row, col++).Value = emp.CostCenterCode ?? "";
                worksheet.Cell(row, col++).Value = emp.CostCenterName ?? "";


                // Datos fiscales (2 columnas)
                worksheet.Cell(row, col++).Value = emp.RUC ?? "";
                worksheet.Cell(row, col++).Value = emp.INSS ?? "";

                // Estado laboral (2 columnas)
                var estadoLaboralCell = worksheet.Cell(row, col++);
                estadoLaboralCell.Value = GetEmploymentStatusText(emp.EmploymentStatus);
                estadoLaboralCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ApplyEmploymentStatusColor(estadoLaboralCell, emp.EmploymentStatus);

                var estadoSistemaCell = worksheet.Cell(row, col++);
                estadoSistemaCell.Value = emp.Estado ? "Activo" : "Inactivo";
                estadoSistemaCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                if (emp.Estado)
                {
                    estadoSistemaCell.Style.Font.FontColor = XLColor.White;
                    estadoSistemaCell.Style.Fill.BackgroundColor = XLColor.Green;
                }
                else
                {
                    estadoSistemaCell.Style.Font.FontColor = XLColor.White;
                    estadoSistemaCell.Style.Fill.BackgroundColor = XLColor.Orange;
                }

                // Condiciones especiales (4 columnas)
                var confianzaCell = worksheet.Cell(row, col++);
                confianzaCell.Value = emp.EsConfianza ? "Sí" : "No";
                confianzaCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                if (emp.EsConfianza)
                {
                    confianzaCell.Style.Fill.BackgroundColor = XLColor.LightYellow;
                }

                // Inicio prueba
                var inicioPruebaCell = worksheet.Cell(row, col++);
                if (emp.InicioPrueba.HasValue)
                {
                    inicioPruebaCell.Value = emp.InicioPrueba.Value;
                    inicioPruebaCell.Style.NumberFormat.Format = "yyyy-mm-dd";
                }

                // Fin prueba
                var finPruebaCell = worksheet.Cell(row, col++);
                if (emp.FinPrueba.HasValue)
                {
                    finPruebaCell.Value = emp.FinPrueba.Value;
                    finPruebaCell.Style.NumberFormat.Format = "yyyy-mm-dd";
                }

                // En prueba (calculado)
                var enPruebaCell = worksheet.Cell(row, col++);
                //var isOnProbationNow = emp.InicioPrueba.HasValue &&
                //                       emp.FinPrueba.HasValue &&
                //                       DateTime.UtcNow >= emp.InicioPrueba.Value &&
                //                       DateTime.UtcNow <= emp.FinPrueba.Value;
           
                var isOnPruebaNow = emp.InicioPrueba.HasValue &&
                                    emp.FinPrueba.HasValue &&
                                    DateTime.Now >= emp.InicioPrueba.Value &&
                                    DateTime.Now <= emp.FinPrueba.Value;

                enPruebaCell.Value = isOnPruebaNow ? "Sí" : "No";
                enPruebaCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                if (isOnPruebaNow)
                {
                    enPruebaCell.Style.Fill.BackgroundColor = XLColor.LightCyan;
                }

                // Trabajador extranjero (3 columnas)
                worksheet.Cell(row, col++).Value = emp.Nacionalidad ?? "";
                worksheet.Cell(row, col++).Value = emp.PermisoTrabajo ?? "";

                var vencimientoCell = worksheet.Cell(row, col++);
                if (emp.VencimientoPermiso.HasValue)
                {
                    vencimientoCell.Value = emp.VencimientoPermiso.Value;
                    vencimientoCell.Style.NumberFormat.Format = "yyyy-mm-dd";

                    // Alerta si el permiso está vencido o por vencer
                    if (emp.VencimientoPermiso.Value < DateTime.Now)
                    {
                        vencimientoCell.Style.Fill.BackgroundColor = XLColor.Red;
                        vencimientoCell.Style.Font.FontColor = XLColor.White;
                    }
                    else if (emp.VencimientoPermiso.Value < DateTime.Now.AddMonths(3))
                    {
                        vencimientoCell.Style.Fill.BackgroundColor = XLColor.Orange;
                        vencimientoCell.Style.Font.FontColor = XLColor.White;
                    }
                }

                // Salario en especie (2 columnas)
                var valorEspecieCell = worksheet.Cell(row, col++);
                if (emp.ValorEspecie.HasValue && emp.ValorEspecie.Value > 0)
                {
                    valorEspecieCell.Value = (double)emp.ValorEspecie.Value;
                    valorEspecieCell.Style.NumberFormat.Format = "#,##0.00";
                }
                worksheet.Cell(row, col++).Value = emp.DescripcionEspecie ?? "";

                // Suspensión (2 columnas)
                var inicioSuspensionCell = worksheet.Cell(row, col++);
                if (emp.InicioSuspension.HasValue)
                {
                    inicioSuspensionCell.Value = emp.InicioSuspension.Value;
                    inicioSuspensionCell.Style.NumberFormat.Format = "yyyy-mm-dd";
                }

                var finSuspensionCell = worksheet.Cell(row, col++);
                if (emp.FinSuspension.HasValue)
                {
                    finSuspensionCell.Value = emp.FinSuspension.Value;
                    finSuspensionCell.Style.NumberFormat.Format = "yyyy-mm-dd";
                }

                // Es reingreso (1 columna)
                var reingresoCell = worksheet.Cell(row, col++);
                reingresoCell.Value = emp.PreviousEmployeeId.HasValue ? "Sí" : "No";
                reingresoCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                if (emp.PreviousEmployeeId.HasValue)
                {
                    reingresoCell.Style.Fill.BackgroundColor = XLColor.LightGreen;
                }

                // Notas (1 columna)
                worksheet.Cell(row, col++).Value = emp.Notes ?? "";
            
            }

            // ✅ Auto-ajustar y filtros
            worksheet.Columns().AdjustToContents();
            worksheet.Range(1, 1, employees.Count + 1, headers.Length).SetAutoFilter();
            worksheet.SheetView.FreezeRows(1);
            worksheet.SheetView.FreezeColumns(3);

            return SaveToBytes(workbook);
        }

        private string GetEmploymentStatusText(int status)
        {
            return status switch
            {
                (int)EmploymentStatus.Active => "Activo",
                (int)EmploymentStatus.Suspended => "Suspendido",
                (int)EmploymentStatus.Terminated => "Terminado",
                _ => "Desconocido"
            };
        }

        private void ApplyEmploymentStatusColor(IXLCell cell, int status)
        {
            switch (status)
            {
                case (int)EmploymentStatus.Active:
                    cell.Style.Font.FontColor = XLColor.White;
                    cell.Style.Fill.BackgroundColor = XLColor.Green;
                    break;
                case (int)EmploymentStatus.Suspended:
                    cell.Style.Font.FontColor = XLColor.White;
                    cell.Style.Fill.BackgroundColor = XLColor.Orange;
                    break;
                case (int)EmploymentStatus.Terminated:
                    cell.Style.Font.FontColor = XLColor.White;
                    cell.Style.Fill.BackgroundColor = XLColor.Red;
                    break;
                default:
                    cell.Style.Fill.BackgroundColor = XLColor.Gray;
                    break;
            }
        }

        private static byte[] SaveToBytes(XLWorkbook workbook)
        {
            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }
    }

    // ✅ DTO actualizado con nuevos campos
    public class EmployeeExportDto
    {
        // Datos básicos
        public string Codigo { get; set; } = "";
        public string Cedula { get; set; } = "";
        public string Nombres { get; set; } = "";
        public string Apellidos { get; set; } = "";
        public string Correo { get; set; } = "";
        public string Telefono { get; set; } = "";
        public DateTime FechaIngreso { get; set; }

        // Datos laborales
        public string? Puesto { get; set; }
        public string? Nivel { get; set; }
        public string? Sucursal { get; set; }
        public string? CodigoSucursal { get; set; }
        public string? TipoContrato { get; set; }
        public decimal SalarioBase { get; set; }

        // Datos fiscales
        public string? RUC { get; set; }
        public string? INSS { get; set; }

        // Estado laboral
        public int EmploymentStatus { get; set; }
        public bool Estado { get; set; }

        // Condiciones especiales
        public bool EsConfianza { get; set; }
        public DateTime? InicioPrueba { get; set; }
        public DateTime? FinPrueba { get; set; }

        // Trabajador extranjero
        public string? Nacionalidad { get; set; }
        public string? PermisoTrabajo { get; set; }
        public DateTime? VencimientoPermiso { get; set; }

        // Salario en especie
        public decimal? ValorEspecie { get; set; }
        public string? DescripcionEspecie { get; set; }

        // Suspensión
        public DateTime? InicioSuspension { get; set; }
        public DateTime? FinSuspension { get; set; }

        // Reingreso
        public Guid? PreviousEmployeeId { get; set; }

        // Notas
        public string? Notes { get; set; }

        public Guid? CostCenterId { get; set; }
        public string CostCenterName { get; set; } = string.Empty;
        public string CostCenterCode { get; set; } = string.Empty;
    }
}