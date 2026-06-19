using Dapper;
using Gestiona360.Payroll.Domain.Interfaces;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using Microsoft.EntityFrameworkCore;
using SqlKata;
using SqlKata.Compilers;


namespace Gestiona360.Payroll.Infrastructure.Persistence.Repositories
{
    public class EmployeeExportRepository : IEmployeeExportRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeExportRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeeExportDto>> GetEmployeesForExportAsync(
            EmployeeExportFilters filters,
            CancellationToken ct = default)
        {
            var query = BuildBaseQuery();
            query = ApplyFilters(query, filters);

            var connection = _context.Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync(ct);

            var compiler = new SqlServerCompiler();
            var compiled = compiler.Compile(query);

            return await connection.QueryAsync<EmployeeExportDto>(
                compiled.Sql,
                compiled.NamedBindings);
        }

        /// <summary>
        /// Construye la consulta base con todos los JOINs y SELECT necesarios.
        /// </summary>
        private static Query BuildBaseQuery()
        {
            return new Query("Employees")
                .LeftJoin("JobGrades", "Employees.JobGradeId", "JobGrades.Id")
                .LeftJoin("JobPositions", "JobGrades.JobPositionId", "JobPositions.Id")
                .LeftJoin("Branches", "Employees.BranchId", "Branches.Id")
                .LeftJoin("CostCenters", "Employees.CostCenterId", "CostCenters.Id")
                .LeftJoin("ContractTypes", "Employees.ContractTypeId", "ContractTypes.Id")
                .LeftJoin("PayrollGroups", "Employees.PayrollGroupId", "PayrollGroups.Id")
                .LeftJoin("Departments", "Employees.DepartmentId", "Departments.Id")
                .LeftJoin("Municipalities", "Employees.MunicipalityId", "Municipalities.Id")
                .LeftJoin("Banks", "Employees.BankId", "Banks.Id")
                .LeftJoin("HealthProviders", "Employees.HealthProviderId", "HealthProviders.Id")
                .Select(
                    // GRUPO 1: Identificación
                    "Employees.Code AS Codigo",
                    "Employees.Identification AS Cedula",
                    "Employees.FirstName AS PrimerNombre",
                    "Employees.SecondName AS SegundoNombre",
                    "Employees.LastName AS PrimerApellido",
                    "Employees.SecondLastName AS SegundoApellido",
                    "Employees.BirthDate AS FechaNacimiento",
                    "Employees.Gender AS Sexo",
                    "Employees.MaritalStatus AS EstadoCivil",
                    // GRUPO 2: Contacto
                    "Employees.Email AS Correo",
                    "Employees.Phone AS Telefono",
                    "Employees.MobilePhone AS Celular",
                    "Employees.Address AS Direccion",
                    "Departments.Name AS Departamento",
                    "Municipalities.Name AS Municipio",
                    // GRUPO 3: Emergencia
                    "Employees.EmergencyContactName AS ContactoEmergencia",
                    "Employees.EmergencyContactPhone AS TelefonoEmergencia",
                    "Employees.EmergencyContactRelationship AS Parentesco",
                    // GRUPO 4: Fiscal
                    "Employees.NORUC AS RUC",
                    "Employees.NOINSS AS INSS",
                    // GRUPO 5: Laboral
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
                    // GRUPO 6: Bancario
                    "Banks.Name AS Banco",
                    "Employees.BankAccountType AS TipoCuenta",
                    "Employees.BankAccountNumber AS CuentaBancaria",
                    "Employees.BankBeneficiaryName AS Beneficiario",
                    // GRUPO 7: Condiciones
                    "Employees.EmploymentStatus AS EmploymentStatus",
                    "Employees.IsActive AS Estado",
                    "Employees.IsTrustEmployee AS EsConfianza",
                    "Employees.UsesTimeClock AS UsaRelojMarcas",
                    "Employees.ProbationStartDate AS InicioPrueba",
                    "Employees.ProbationEndDate AS FinPrueba",
                    // GRUPO 8: Extranjero
                    "Employees.Nationality AS Nacionalidad",
                    "Employees.WorkPermitNumber AS PermisoTrabajo",
                    "Employees.WorkPermitExpirationDate AS VencimientoPermiso",
                    // GRUPO 9: Especie
                    "Employees.BenefitsInKindValue AS ValorEspecie",
                    "Employees.BenefitsInKindDescription AS DescripcionEspecie",
                    // GRUPO 10: Suspensión
                    "Employees.SuspensionStartDate AS InicioSuspension",
                    "Employees.SuspensionEndDate AS FinSuspension",
                    // GRUPO 11: Reingreso
                    "Employees.PreviousEmployeeId AS PreviousEmployeeId",
                    // GRUPO 12: Notas
                    "Employees.Notes AS Notes"
                )
                .OrderBy("Employees.FirstName")
                .OrderBy("Employees.LastName");
        }

        /// <summary>
        /// Aplica los filtros dinámicos a la consulta.
        /// </summary>
        private static Query ApplyFilters(Query query, EmployeeExportFilters filters)
        {
            if (!string.IsNullOrWhiteSpace(filters.Search))
            {
                var searchTerm = $"%{filters.Search}%";
                query = query.Where(q => q
                    .OrWhereLike("Employees.FirstName", searchTerm)
                    .OrWhereLike("Employees.LastName", searchTerm)
                    .OrWhereLike("Employees.Identification", searchTerm)
                    .OrWhereLike("Employees.Code", searchTerm)
                    .OrWhereLike("Employees.Email", searchTerm)
                    .OrWhereLike("Employees.NORUC", searchTerm)
                    .OrWhereLike("Employees.NOINSS", searchTerm));
            }

            if (filters.BranchId.HasValue)
                query = query.Where("Employees.BranchId", filters.BranchId.Value);

            if (filters.ContractTypeId.HasValue)
                query = query.Where("Employees.ContractTypeId", filters.ContractTypeId.Value);

            if (filters.EmploymentStatus.HasValue)
                query = query.Where("Employees.EmploymentStatus", filters.EmploymentStatus.Value);

            if (filters.JobPositionId.HasValue)
                query = query.Where("JobGrades.JobPositionId", filters.JobPositionId.Value);

            if (filters.IsTrustEmployee.HasValue)
                query = query.Where("Employees.IsTrustEmployee", filters.IsTrustEmployee.Value);

            if (filters.IsForeignWorker.HasValue)
            {
                query = filters.IsForeignWorker.Value
                    ? query.WhereNotNull("Employees.Nationality")
                    : query.WhereNull("Employees.Nationality");
            }

            if (filters.PayrollGroupId.HasValue)
                query = query.Where("Employees.PayrollGroupId", filters.PayrollGroupId.Value);

            if (filters.IsOnProbation.HasValue)
            {
                var now = DateTime.UtcNow;
                query = filters.IsOnProbation.Value
                    ? query.WhereNotNull("Employees.ProbationStartDate")
                           .WhereNotNull("Employees.ProbationEndDate")
                           .Where("Employees.ProbationStartDate", "<=", now)
                           .Where("Employees.ProbationEndDate", ">=", now)
                    : query.Where(q => q
                        .OrWhereNull("Employees.ProbationStartDate")
                        .OrWhereNull("Employees.ProbationEndDate")
                        .OrWhere("Employees.ProbationStartDate", ">", now)
                        .OrWhere("Employees.ProbationEndDate", "<", now));
            }

            if (filters.IsRehire.HasValue)
            {
                query = filters.IsRehire.Value
                    ? query.WhereNotNull("Employees.PreviousEmployeeId")
                    : query.WhereNull("Employees.PreviousEmployeeId");
            }

            return query;
        }
    }
}
