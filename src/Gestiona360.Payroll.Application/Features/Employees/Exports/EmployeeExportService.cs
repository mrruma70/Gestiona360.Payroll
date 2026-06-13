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
            int? employmentStatus = null,
            Guid? jobPositionId = null,
            bool? isTrustEmployee = null,
            bool? isForeignWorker = null,
            bool? isOnProbation = null,
            bool? isRehire = null,
            Guid? payrollGroupId = null)
        {
            // ✅ Construir query con SqlKata (filtros dinámicos)
            var query = new Query("Employees")
                // ═══════════════════════════════════════════════════════════════
                // JOINS
                // ═══════════════════════════════════════════════════════════════
                .LeftJoin("JobGrades", "Employees.JobGradeId", "JobGrades.Id")
                .LeftJoin("JobPositions", "JobGrades.JobPositionId", "JobPositions.Id")
                .LeftJoin("Branches", "Employees.BranchId", "Branches.Id")
                .LeftJoin("CostCenters", "Employees.CostCenterId", "CostCenters.Id")
                .LeftJoin("ContractTypes", "Employees.ContractTypeId", "ContractTypes.Id")
                .LeftJoin("PayrollGroups", "Employees.PayrollGroupId", "PayrollGroups.Id")
                .LeftJoin("Departments", "Employees.DepartmentId", "Departments.Id")           // ✅ NUEVO
                .LeftJoin("Municipalities", "Employees.MunicipalityId", "Municipalities.Id")   // ✅ NUEVO
                .LeftJoin("Banks", "Employees.BankId", "Banks.Id")                             // ✅ NUEVO
                .LeftJoin("HealthProviders", "Employees.HealthProviderId", "HealthProviders.Id") // ✅ NUEVO

                // ═══════════════════════════════════════════════════════════════
                // SELECT - ORGANIZADO POR GRUPOS DEL MAESTRO
                // ═══════════════════════════════════════════════════════════════
                .Select(
                    // ── GRUPO 1: IDENTIFICACIÓN BÁSICA ──
                    "Employees.Code AS Codigo",
                    "Employees.Identification AS Cedula",
                    "Employees.FirstName AS PrimerNombre",
                    "Employees.SecondName AS SegundoNombre",
                    "Employees.LastName AS PrimerApellido",
                    "Employees.SecondLastName AS SegundoApellido",
                    "Employees.BirthDate AS FechaNacimiento",
                    "Employees.Gender AS Sexo",
                    "Employees.MaritalStatus AS EstadoCivil",

                    // ── GRUPO 2: CONTACTO Y DOMICILIO ──
                    "Employees.Email AS Correo",
                    "Employees.Phone AS Telefono",
                    "Employees.MobilePhone AS Celular",
                    "Employees.Address AS Direccion",
                    "Departments.Name AS Departamento",
                    "Municipalities.Name AS Municipio",

                    // ── GRUPO 3: CONTACTO DE EMERGENCIA ──
                    "Employees.EmergencyContactName AS ContactoEmergencia",
                    "Employees.EmergencyContactPhone AS TelefonoEmergencia",
                    "Employees.EmergencyContactRelationship AS Parentesco",

                    // ── GRUPO 4: DATOS FISCALES ──
                    "Employees.NORUC AS RUC",
                    "Employees.NOINSS AS INSS",

                    // ── GRUPO 5: DATOS LABORALES ──
                    "Employees.HireDate AS FechaIngreso",
                    "Employees.FirstHireDate AS FechaPrimerIngreso",
                    "JobPositions.Name AS Puesto",
                    "JobGrades.Name AS Nivel",
                    "Branches.Name AS Sucursal",
                    "Branches.Code AS CodigoSucursal",
                    "ContractTypes.Name AS TipoContrato",
                    "Employees.BaseSalary AS SalarioBase",
                    "CostCenters.Code AS CostCenterCode",
                    "CostCenters.Name AS CostCenterName",
                    "PayrollGroups.Name AS GrupoNomina",
                    "HealthProviders.Name AS ProveedorSalud",

                    // ── GRUPO 6: DATOS BANCARIOS ──
                    "Banks.Name AS Banco",
                    "Employees.BankAccountType AS TipoCuenta",
                    "Employees.BankAccountNumber AS CuentaBancaria",
                    "Employees.BankBeneficiaryName AS Beneficiario",

                    // ── GRUPO 7: CONDICIONES ESPECIALES ──
                    "Employees.EmploymentStatus AS EmploymentStatus",
                    "Employees.IsActive AS Estado",
                    "Employees.IsTrustEmployee AS EsConfianza",
                    "Employees.UsesTimeClock AS UsaRelojMarcas",
                    "Employees.ProbationStartDate AS InicioPrueba",
                    "Employees.ProbationEndDate AS FinPrueba",

                    // ── GRUPO 8: TRABAJADOR EXTRANJERO ──
                    "Employees.Nationality AS Nacionalidad",
                    "Employees.WorkPermitNumber AS PermisoTrabajo",
                    "Employees.WorkPermitExpirationDate AS VencimientoPermiso",

                    // ── GRUPO 9: BENEFICIOS EN ESPECIE ──
                    "Employees.BenefitsInKindValue AS ValorEspecie",
                    "Employees.BenefitsInKindDescription AS DescripcionEspecie",

                    // ── GRUPO 10: SUSPENSIÓN ──
                    "Employees.SuspensionStartDate AS InicioSuspension",
                    "Employees.SuspensionEndDate AS FinSuspension",

                    // ── GRUPO 11: REINGRESO ──
                    "Employees.PreviousEmployeeId AS PreviousEmployeeId",

                    // ── GRUPO 12: NOTAS ──
                    "Employees.Notes AS Notas"
                );

            // ═══════════════════════════════════════════════════════════════
            // FILTROS DINÁMICOS
            // ═══════════════════════════════════════════════════════════════

            if (!string.IsNullOrWhiteSpace(search))
            {
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

            if (employmentStatus.HasValue)
                query = query.Where("Employees.EmploymentStatus", employmentStatus.Value);

            if (jobPositionId.HasValue)
                query = query.Where("JobGrades.JobPositionId", jobPositionId.Value);

            if (isTrustEmployee.HasValue)
                query = query.Where("Employees.IsTrustEmployee", isTrustEmployee.Value);

            if (isForeignWorker.HasValue)
            {
                if (isForeignWorker.Value)
                    query = query.WhereNotNull("Employees.Nationality");
                else
                    query = query.WhereNull("Employees.Nationality");
            }

            if (payrollGroupId.HasValue)
                query = query.Where("Employees.PayrollGroupId", payrollGroupId.Value);

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

            if (isRehire.HasValue)
            {
                if (isRehire.Value)
                    query = query.WhereNotNull("Employees.PreviousEmployeeId");
                else
                    query = query.WhereNull("Employees.PreviousEmployeeId");
            }

            query = query.OrderBy("Employees.FirstName").OrderBy("Employees.LastName");

            // ═══════════════════════════════════════════════════════════════
            // EJECUTAR QUERY
            // ═══════════════════════════════════════════════════════════════

            var connection = _context.Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();

            var compiler = new SqlKata.Compilers.SqlServerCompiler();
            var compiled = compiler.Compile(query);

            var employees = await connection.QueryAsync<EmployeeExportDto>(
                compiled.Sql,
                compiled.NamedBindings
            );

            return GenerateExcel(employees.ToList());
        }

        // ═══════════════════════════════════════════════════════════════
        // GENERAR EXCEL
        // ═══════════════════════════════════════════════════════════════

        private byte[] GenerateExcel(List<EmployeeExportDto> employees)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Empleados");

            if (!employees.Any())
            {
                worksheet.Cell(1, 1).Value = "No hay datos para mostrar";
                return SaveToBytes(workbook);
            }

            // ═══════════════════════════════════════════════════════════════
            // ENCABEZADOS ORGANIZADOS POR GRUPOS
            // ═══════════════════════════════════════════════════════════════

            var headers = new[]
            {
                // GRUPO 1: IDENTIFICACIÓN BÁSICA (9 columnas)
                "Código", "Cédula", "Primer Nombre", "Segundo Nombre",
                "Primer Apellido", "Segundo Apellido",
                "Fecha Nacimiento", "Sexo", "Estado Civil",

                // GRUPO 2: CONTACTO Y DOMICILIO (6 columnas)
                "Correo", "Teléfono", "Celular",
                "Dirección", "Departamento", "Municipio",

                // GRUPO 3: CONTACTO DE EMERGENCIA (3 columnas)
                "Contacto Emergencia", "Teléfono Emergencia", "Parentesco",

                // GRUPO 4: DATOS FISCALES (2 columnas)
                "RUC", "INSS",

                // GRUPO 5: DATOS LABORALES (11 columnas)
                "Fecha Ingreso", "Fecha Primer Ingreso",
                "Puesto", "Nivel", "Sucursal", "Código Sucursal",
                "Tipo Contrato", "Salario Base",
                "Código Centro Costo", "Centro de Costo", "Grupo de Nómina",
                "Proveedor de Salud",

                // GRUPO 6: DATOS BANCARIOS (4 columnas)
                "Banco", "Tipo Cuenta", "Cuenta Bancaria", "Beneficiario",

                // GRUPO 7: CONDICIONES ESPECIALES (6 columnas)
                "Estado Laboral", "Estado Sistema",
                "Empleado Confianza", "Usa Reloj Marcas",
                "Inicio Período Prueba", "Fin Período Prueba",

                // GRUPO 8: TRABAJADOR EXTRANJERO (3 columnas)
                "Nacionalidad", "Permiso Trabajo", "Vencimiento Permiso",

                // GRUPO 9: BENEFICIOS EN ESPECIE (2 columnas)
                "Valor en Especie", "Descripción Especie",

                // GRUPO 10: SUSPENSIÓN (2 columnas)
                "Inicio Suspensión", "Fin Suspensión",

                // GRUPO 11: REINGRESO (1 columna)
                "Es Reingreso",

                // GRUPO 12: NOTAS (1 columna)
                "Notas"
            };

            // ═══════════════════════════════════════════════════════════════
            // ESTILO DE ENCABEZADOS CON COLORES POR GRUPO
            // ═══════════════════════════════════════════════════════════════

            // Definir colores por grupo
            var groupColors = new Dictionary<int, XLColor>
            {
                { 0, XLColor.LightSteelBlue },    // Identificación (cols 1-9)
                { 1, XLColor.LightGreen },         // Contacto (cols 10-15)
                { 2, XLColor.LightPink },          // Emergencia (cols 16-18)
                { 3, XLColor.LightYellow },        // Fiscal (cols 19-20)
                { 4, XLColor.LightCyan },          // Laboral (cols 21-32)
                { 5, XLColor.LightBlue },          // Bancario (cols 33-36)
                { 6, XLColor.Lavender },           // Condiciones (cols 37-42)
                { 7, XLColor.Orange },             // Extranjero (cols 43-45)
                { 8, XLColor.LavenderFloral },     // Especie (cols 46-47)
                { 9, XLColor.Salmon },             // Suspensión (cols 48-49)
                { 10, XLColor.LightGreen },        // Reingreso (col 50)
                { 11, XLColor.WhiteSmoke }         // Notas (col 51)
            };

            // Definir rangos de columnas por grupo
            var groupRanges = new (int start, int end, int groupKey)[]
            {
                (1, 9, 0),      // Identificación
                (10, 15, 1),    // Contacto
                (16, 18, 2),    // Emergencia
                (19, 20, 3),    // Fiscal
                (21, 32, 4),    // Laboral
                (33, 36, 5),    // Bancario
                (37, 42, 6),    // Condiciones
                (43, 45, 7),    // Extranjero
                (46, 47, 8),    // Especie
                (48, 49, 9),    // Suspensión
                (50, 50, 10),   // Reingreso
                (51, 51, 11)    // Notas
            };

            // Aplicar colores a encabezados
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Alignment.WrapText = true;

                // Asignar color según grupo
                foreach (var (start, end, groupKey) in groupRanges)
                {
                    if (i + 1 >= start && i + 1 <= end)
                    {
                        cell.Style.Fill.BackgroundColor = groupColors[groupKey];
                        break;
                    }
                }
            }

            // ═══════════════════════════════════════════════════════════════
            // DATOS - MAPEO DE COLUMNAS
            // ═══════════════════════════════════════════════════════════════

            for (int rowIdx = 0; rowIdx < employees.Count; rowIdx++)
            {
                var emp = employees[rowIdx];
                var row = rowIdx + 2;
                var col = 1;

                // ── GRUPO 1: IDENTIFICACIÓN BÁSICA (9 columnas) ──
                worksheet.Cell(row, col++).Value = emp.Codigo;
                worksheet.Cell(row, col++).Value = emp.Cedula;
                worksheet.Cell(row, col++).Value = emp.PrimerNombre ?? "";
                worksheet.Cell(row, col++).Value = emp.SegundoNombre ?? "";
                worksheet.Cell(row, col++).Value = emp.PrimerApellido ?? "";
                worksheet.Cell(row, col++).Value = emp.SegundoApellido ?? "";

                var fechaNacCell = worksheet.Cell(row, col++);
                if (emp.FechaNacimiento.HasValue)
                {
                    fechaNacCell.Value = emp.FechaNacimiento.Value;
                    fechaNacCell.Style.NumberFormat.Format = "yyyy-mm-dd";
                }

                worksheet.Cell(row, col++).Value = GetGenderText(emp.Sexo);
                worksheet.Cell(row, col++).Value = GetMaritalStatusText(emp.EstadoCivil);

                // ── GRUPO 2: CONTACTO Y DOMICILIO (6 columnas) ──
                worksheet.Cell(row, col++).Value = emp.Correo ?? "";
                worksheet.Cell(row, col++).Value = emp.Telefono ?? "";
                worksheet.Cell(row, col++).Value = emp.Celular ?? "";
                worksheet.Cell(row, col++).Value = emp.Direccion ?? "";
                worksheet.Cell(row, col++).Value = emp.Departamento ?? "";
                worksheet.Cell(row, col++).Value = emp.Municipio ?? "";

                // ── GRUPO 3: CONTACTO DE EMERGENCIA (3 columnas) ──
                worksheet.Cell(row, col++).Value = emp.ContactoEmergencia ?? "";
                worksheet.Cell(row, col++).Value = emp.TelefonoEmergencia ?? "";
                worksheet.Cell(row, col++).Value = emp.Parentesco ?? "";

                // ── GRUPO 4: DATOS FISCALES (2 columnas) ──
                worksheet.Cell(row, col++).Value = emp.RUC ?? "";
                worksheet.Cell(row, col++).Value = emp.INSS ?? "";

                // ── GRUPO 5: DATOS LABORALES (12 columnas) ──
                var fechaIngCell = worksheet.Cell(row, col++);
                fechaIngCell.Value = emp.FechaIngreso;
                fechaIngCell.Style.NumberFormat.Format = "yyyy-mm-dd";

                var fechaPrimIngCell = worksheet.Cell(row, col++);
                if (emp.FechaPrimerIngreso.HasValue)
                {
                    fechaPrimIngCell.Value = emp.FechaPrimerIngreso.Value;
                    fechaPrimIngCell.Style.NumberFormat.Format = "yyyy-mm-dd";
                }

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
                worksheet.Cell(row, col++).Value = emp.GrupoNomina ?? "No asignado";
                worksheet.Cell(row, col++).Value = emp.ProveedorSalud ?? "No asignado";

                // ── GRUPO 6: DATOS BANCARIOS (4 columnas) ──
                worksheet.Cell(row, col++).Value = emp.Banco ?? "Sin cuenta";
                worksheet.Cell(row, col++).Value = emp.TipoCuenta ?? "";
                worksheet.Cell(row, col++).Value = emp.CuentaBancaria ?? "";
                worksheet.Cell(row, col++).Value = emp.Beneficiario ?? "";

                // ── GRUPO 7: CONDICIONES ESPECIALES (6 columnas) ──
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

                var confianzaCell = worksheet.Cell(row, col++);
                confianzaCell.Value = emp.EsConfianza ? "Sí" : "No";
                confianzaCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                if (emp.EsConfianza)
                    confianzaCell.Style.Fill.BackgroundColor = XLColor.LightYellow;

                var relojCell = worksheet.Cell(row, col++);
                relojCell.Value = emp.UsaRelojMarcas ? "Sí" : "No";
                relojCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                if (emp.UsaRelojMarcas)
                    relojCell.Style.Fill.BackgroundColor = XLColor.LightCyan;

                var inicioPruebaCell = worksheet.Cell(row, col++);
                if (emp.InicioPrueba.HasValue)
                {
                    inicioPruebaCell.Value = emp.InicioPrueba.Value;
                    inicioPruebaCell.Style.NumberFormat.Format = "yyyy-mm-dd";
                }

                var finPruebaCell = worksheet.Cell(row, col++);
                if (emp.FinPrueba.HasValue)
                {
                    finPruebaCell.Value = emp.FinPrueba.Value;
                    finPruebaCell.Style.NumberFormat.Format = "yyyy-mm-dd";
                }

                // ── GRUPO 8: TRABAJADOR EXTRANJERO (3 columnas) ──
                worksheet.Cell(row, col++).Value = emp.Nacionalidad ?? "";
                worksheet.Cell(row, col++).Value = emp.PermisoTrabajo ?? "";

                var vencimientoCell = worksheet.Cell(row, col++);
                if (emp.VencimientoPermiso.HasValue)
                {
                    vencimientoCell.Value = emp.VencimientoPermiso.Value;
                    vencimientoCell.Style.NumberFormat.Format = "yyyy-mm-dd";

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

                // ── GRUPO 9: BENEFICIOS EN ESPECIE (2 columnas) ──
                var valorEspecieCell = worksheet.Cell(row, col++);
                if (emp.ValorEspecie.HasValue && emp.ValorEspecie.Value > 0)
                {
                    valorEspecieCell.Value = (double)emp.ValorEspecie.Value;
                    valorEspecieCell.Style.NumberFormat.Format = "#,##0.00";
                }
                worksheet.Cell(row, col++).Value = emp.DescripcionEspecie ?? "";

                // ── GRUPO 10: SUSPENSIÓN (2 columnas) ──
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

                // ── GRUPO 11: REINGRESO (1 columna) ──
                var reingresoCell = worksheet.Cell(row, col++);
                reingresoCell.Value = emp.PreviousEmployeeId.HasValue ? "Sí" : "No";
                reingresoCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                if (emp.PreviousEmployeeId.HasValue)
                    reingresoCell.Style.Fill.BackgroundColor = XLColor.LightGreen;

                // ── GRUPO 12: NOTAS (1 columna) ──
                worksheet.Cell(row, col++).Value = emp.Notes ?? "";
            }

            // ═══════════════════════════════════════════════════════════════
            // FORMATO FINAL
            // ═══════════════════════════════════════════════════════════════

            worksheet.Columns().AdjustToContents();
            worksheet.Range(1, 1, employees.Count + 1, headers.Length).SetAutoFilter();
            worksheet.SheetView.FreezeRows(1);
            worksheet.SheetView.FreezeColumns(3);

            // Agregar fila de encabezados de grupo (fila 0)
            InsertGroupHeaders(worksheet, headers.Length);

            return SaveToBytes(workbook);
        }

        // ═══════════════════════════════════════════════════════════════
        // ENCABEZADOS DE GRUPO (FILA SUPERIOR)
        // ═══════════════════════════════════════════════════════════════

        private void InsertGroupHeaders(IXLWorksheet worksheet, int totalColumns)
        {
            // Insertar fila en la posición 1
            worksheet.Row(1).InsertRowsAbove(1);

            // Definir grupos con sus rangos de columnas
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

            // Ajustar altura de la fila de grupos
            worksheet.Row(1).Height = 25;
        }

        // ═══════════════════════════════════════════════════════════════
        // MÉTODOS HELPER
        // ═══════════════════════════════════════════════════════════════

        private string GetGenderText(string? gender) => gender switch
        {
            "M" => "Masculino",
            "F" => "Femenino",
            _ => ""
        };

        private string GetMaritalStatusText(string? status) => status switch
        {
            "S" => "Soltero/a",
            "C" => "Casado/a",
            "U" => "Unión libre",
            "D" => "Divorciado/a",
            "V" => "Viudo/a",
            _ => ""
        };

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

    // ═══════════════════════════════════════════════════════════════
    // DTO ACTUALIZADO CON TODOS LOS CAMPOS DEL MAESTRO
    // ═══════════════════════════════════════════════════════════════

    public class EmployeeExportDto
    {
        // GRUPO 1: IDENTIFICACIÓN BÁSICA
        public string Codigo { get; set; } = "";
        public string Cedula { get; set; } = "";
        public string? PrimerNombre { get; set; }
        public string? SegundoNombre { get; set; }
        public string? PrimerApellido { get; set; }
        public string? SegundoApellido { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string? Sexo { get; set; }
        public string? EstadoCivil { get; set; }

        // GRUPO 2: CONTACTO Y DOMICILIO
        public string? Correo { get; set; }
        public string? Telefono { get; set; }
        public string? Celular { get; set; }
        public string? Direccion { get; set; }
        public string? Departamento { get; set; }
        public string? Municipio { get; set; }

        // GRUPO 3: CONTACTO DE EMERGENCIA
        public string? ContactoEmergencia { get; set; }
        public string? TelefonoEmergencia { get; set; }
        public string? Parentesco { get; set; }

        // GRUPO 4: DATOS FISCALES
        public string? RUC { get; set; }
        public string? INSS { get; set; }

        // GRUPO 5: DATOS LABORALES
        public DateTime FechaIngreso { get; set; }
        public DateTime? FechaPrimerIngreso { get; set; }
        public string? Puesto { get; set; }
        public string? Nivel { get; set; }
        public string? Sucursal { get; set; }
        public string? CodigoSucursal { get; set; }
        public string? TipoContrato { get; set; }
        public decimal SalarioBase { get; set; }
        public string? CostCenterCode { get; set; }
        public string? CostCenterName { get; set; }
        public string? GrupoNomina { get; set; }
        public string? ProveedorSalud { get; set; }

        // GRUPO 6: DATOS BANCARIOS
        public string? Banco { get; set; }
        public string? TipoCuenta { get; set; }
        public string? CuentaBancaria { get; set; }
        public string? Beneficiario { get; set; }

        // GRUPO 7: CONDICIONES ESPECIALES
        public int EmploymentStatus { get; set; }
        public bool Estado { get; set; }
        public bool EsConfianza { get; set; }
        public bool UsaRelojMarcas { get; set; }
        public DateTime? InicioPrueba { get; set; }
        public DateTime? FinPrueba { get; set; }

        // GRUPO 8: TRABAJADOR EXTRANJERO
        public string? Nacionalidad { get; set; }
        public string? PermisoTrabajo { get; set; }
        public DateTime? VencimientoPermiso { get; set; }

        // GRUPO 9: BENEFICIOS EN ESPECIE
        public decimal? ValorEspecie { get; set; }
        public string? DescripcionEspecie { get; set; }

        // GRUPO 10: SUSPENSIÓN
        public DateTime? InicioSuspension { get; set; }
        public DateTime? FinSuspension { get; set; }

        // GRUPO 11: REINGRESO
        public Guid? PreviousEmployeeId { get; set; }

        // GRUPO 12: NOTAS
        public string? Notes { get; set; }
    }
}