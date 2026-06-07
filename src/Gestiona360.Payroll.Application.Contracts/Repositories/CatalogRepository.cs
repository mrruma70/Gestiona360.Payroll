using System.Data.SqlClient;
using Dapper;
using Gestiona360.Payroll.Application.Contracts.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Gestiona360.Payroll.Application.Contracts.Repositories
{
    /// <summary>
    /// Repositorio de Catálogos implementado con Dapper
    /// Optimizado para SQL Server con parámetros seguros y lazy loading
    /// </summary>
    public class CatalogRepository : ICatalogRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<CatalogRepository> _logger;

        // Mapeo de tipos de catálogo a tablas SQL
        private readonly Dictionary<string, string> _catalogTableMap = new()
        {
            { "PayrollFrequencies", "PayrollFrequencies" },
            { "ContractTypes", "ContractTypes" },
            { "OccupationalRisks", "OccupationalRisks" },
            { "INSSConfigs", "INSSConfigs" },
            { "IR_TaxBrackets", "IR_TaxBrackets" },
            { "MinimumWages", "MinimumWages" },
            { "HolidayCalendars", "HolidayCalendars" },
            { "Banks", "Banks" }
        };

        public CatalogRepository(IConfiguration configuration, ILogger<CatalogRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");
            _logger = logger;
        }

        // ─ OPERACIONES CRUD

        /// <summary>
        /// Obtiene todos los catálogos de una tabla
        /// Optimización: Se cachea a nivel de aplicación si es necesario
        /// </summary>
        public async Task<List<CatalogItemDto>> GetAllAsync(string catalogType)
        {
            if (!ValidateCatalogType(catalogType))
                throw new ArgumentException($"Tipo de catálogo no válido: {catalogType}");

            var table = _catalogTableMap[catalogType];
            var sql = $@"
                SELECT 
                    Id,
                    Name,
                    Code,
                    Description,
                    Value = CAST(NULL AS NVARCHAR(50)),
                    IsActive,
                    CreatedAt,
                    UpdatedAt
                FROM [{table}]
                WHERE IsActive = 1
                ORDER BY Name ASC
            ";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    var results = (await connection.QueryAsync<CatalogItemDto>(sql)).ToList();

                    _logger.LogInformation($"Obtenidos {results.Count} registros de {catalogType}");
                    return results;
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError($"Error de BD al obtener catálogos: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Obtiene un catálogo por ID
        /// Optimización: Índice en PK garantiza búsqueda O(1)
        /// </summary>
        public async Task<CatalogItemDto> GetByIdAsync(string catalogType, int id)
        {
            if (!ValidateCatalogType(catalogType))
                throw new ArgumentException($"Tipo de catálogo no válido: {catalogType}");

            var table = _catalogTableMap[catalogType];
            var sql = $@"
                SELECT 
                    Id,
                    Name,
                    Code,
                    Description,
                    Value = CAST(NULL AS NVARCHAR(50)),
                    IsActive,
                    CreatedAt,
                    UpdatedAt
                FROM [{table}]
                WHERE Id = @Id
            ";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    return await connection.QueryFirstOrDefaultAsync<CatalogItemDto>(sql, new { Id = id });
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError($"Error al obtener catálogo {id}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Inserta un nuevo catálogo
        /// Retorna el ID generado
        /// </summary>
        public async Task<int> InsertAsync(string catalogType, CatalogItemDto catalog)
        {
            if (!ValidateCatalogType(catalogType))
                throw new ArgumentException($"Tipo de catálogo no válido: {catalogType}");

            // Validación de negocio
            if (string.IsNullOrWhiteSpace(catalog.Name))
                throw new ArgumentException("El nombre es requerido");

            if (string.IsNullOrWhiteSpace(catalog.Code))
                throw new ArgumentException("El código es requerido");

            var table = _catalogTableMap[catalogType];

            // SQL genérico que funciona con todas las tablas de catálogos
            var sql = $@"
                INSERT INTO [{table}] (Name, Code, Description, Value, IsActive, CreatedAt, UpdatedAt)
                VALUES (@Name, @Code, @Description, @Value, @IsActive, @CreatedAt, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as int);
            ";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var newId = await connection.QuerySingleAsync<int>(sql, new
                    {
                        catalog.Name,
                        catalog.Code,
                        catalog.Description,
                        catalog.Value,
                        catalog.IsActive,
                        CreatedAt = catalog.CreatedAt == DateTime.MinValue ? DateTime.UtcNow : catalog.CreatedAt,
                        catalog.UpdatedAt
                    });

                    _logger.LogInformation($"Catálogo insertado en {catalogType} con ID: {newId}");
                    return newId;
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError($"Error al insertar catálogo: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza un catálogo existente
        /// Retorna verdadero si fue actualizado
        /// </summary>
        public async Task<bool> UpdateAsync(string catalogType, CatalogItemDto catalog)
        {
            if (!ValidateCatalogType(catalogType))
                throw new ArgumentException($"Tipo de catálogo no válido: {catalogType}");

            if (catalog.Id <= 0)
                throw new ArgumentException("ID inválido");

            var table = _catalogTableMap[catalogType];

            var sql = $@"
                UPDATE [{table}]
                SET 
                    Name = @Name,
                    Code = @Code,
                    Description = @Description,
                    Value = @Value,
                    IsActive = @IsActive,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id
            ";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var rowsAffected = await connection.ExecuteAsync(sql, new
                    {
                        catalog.Id,
                        catalog.Name,
                        catalog.Code,
                        catalog.Description,
                        catalog.Value,
                        catalog.IsActive,
                        UpdatedAt = DateTime.UtcNow
                    });

                    _logger.LogInformation($"Catálogo {catalog.Id} actualizado en {catalogType}");
                    return rowsAffected > 0;
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError($"Error al actualizar catálogo: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Elimina lógicamente un catálogo (soft delete)
        /// </summary>
        public async Task<bool> DeleteAsync(string catalogType, int id)
        {
            if (!ValidateCatalogType(catalogType))
                throw new ArgumentException($"Tipo de catálogo no válido: {catalogType}");

            var table = _catalogTableMap[catalogType];

            var sql = $@"
                UPDATE [{table}]
                SET IsActive = 0, UpdatedAt = @UpdatedAt
                WHERE Id = @Id
            ";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var rowsAffected = await connection.ExecuteAsync(sql, new
                    {
                        Id = id,
                        UpdatedAt = DateTime.UtcNow
                    });

                    _logger.LogInformation($"Catálogo {id} desactivado en {catalogType}");
                    return rowsAffected > 0;
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError($"Error al eliminar catálogo: {ex.Message}");
                throw;
            }
        }

        // ─ BÚSQUEDA Y FILTRADO

        /// <summary>
        /// Búsqueda full-text con filtros
        /// Optimización: Usa índices de SQL Server
        /// </summary>
        public async Task<List<CatalogItemDto>> SearchAsync(string catalogType, string searchTerm, bool? isActive = null)
        {
            if (!ValidateCatalogType(catalogType))
                throw new ArgumentException($"Tipo de catálogo no válido: {catalogType}");

            var table = _catalogTableMap[catalogType];
            var searchPattern = $"%{searchTerm}%";

            // Construir SQL dinámico (seguro porque table viene de un diccionario interno)
            var sql = $@"
        SELECT 
            Id,
            Name,
            Code,
            Description,
            Value = CAST(NULL AS NVARCHAR(50)),  -- Si la tabla no tiene Value, se devuelve NULL
            IsActive,
            CreatedAt,
            UpdatedAt
        FROM [{table}]
        WHERE 
            (Name LIKE @SearchPattern 
             OR Code LIKE @SearchPattern 
             OR Description LIKE @SearchPattern)
            {(isActive.HasValue ? "AND IsActive = @IsActive" : "")}
        ORDER BY Name ASC";

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var parameters = new { SearchPattern = searchPattern, IsActive = isActive };
                var results = (await connection.QueryAsync<CatalogItemDto>(sql, parameters)).ToList();

                _logger.LogInformation($"Búsqueda en {catalogType}: encontrados {results.Count} registros");
                return results;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error en búsqueda para {CatalogType}", catalogType);
                throw;
            }
        }

        /// <summary>
        /// Valida si un código es único
        /// Usado para validación de integridad
        /// </summary>
        public async Task<bool> IsCodeUniqueAsync(string catalogType, string code, int? excludeId = null)
        {
            if (!ValidateCatalogType(catalogType))
                throw new ArgumentException($"Tipo de catálogo no válido: {catalogType}");

            var table = _catalogTableMap[catalogType];

            var sql = $@"
                SELECT COUNT(1)
                FROM [{table}]
                WHERE Code = @Code
                {(excludeId.HasValue ? "AND Id != @ExcludeId" : "")}
            ";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var count = await connection.QuerySingleAsync<int>(sql, new
                    {
                        Code = code,
                        ExcludeId = excludeId ?? 0
                    });

                    return count == 0; // Unique if count is 0
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError($"Error al validar unicidad de código: {ex.Message}");
                throw;
            }
        }

        // ─ VALIDACIONES

        /// <summary>
        /// Obtiene el conteo de catálogos
        /// </summary>
        public async Task<int> GetCountAsync(string catalogType, bool? isActive = null)
        {
            if (!ValidateCatalogType(catalogType))
                throw new ArgumentException($"Tipo de catálogo no válido: {catalogType}");

            var table = _catalogTableMap[catalogType];

            var sql = $@"
                SELECT COUNT(1)
                FROM [{table}]
                {(isActive.HasValue ? "WHERE IsActive = @IsActive" : "")}
            ";

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    return await connection.QuerySingleAsync<int>(sql,
                        isActive.HasValue ? new { IsActive = isActive.Value } : null);
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError($"Error al contar registros: {ex.Message}");
                throw;
            }
        }

        // ─ UTILIDADES PRIVADAS

        /// <summary>
        /// Valida que el tipo de catálogo sea soportado
        /// </summary>
        private bool ValidateCatalogType(string catalogType)
        {
            return !string.IsNullOrWhiteSpace(catalogType) && _catalogTableMap.ContainsKey(catalogType);
        }
    }

}
