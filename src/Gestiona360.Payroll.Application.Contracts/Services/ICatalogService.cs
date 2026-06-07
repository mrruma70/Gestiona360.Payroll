using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Application.Contracts.DTOs;

namespace Gestiona360.Payroll.Application.Contracts.Services
{
    /// <summary>
    /// Interfaz de servicio para operaciones CRUD de catálogos
    /// Implementa patrón Repository + Unit of Work
    /// Compatible con Dapper y SQL Server
    /// </summary>
    public interface ICatalogService
    {
        // ── OPERACIONES BÁSICAS

        /// <summary>
        /// Obtiene todos los catálogos de una categoría específica
        /// </summary>
        /// <param name="catalogType">Tipo de catálogo (PayrollFrequencies, OccupationalRisks, etc.)</param>
        /// <returns>Lista de items del catálogo</returns>
        Task<List<CatalogItemDto>> GetCatalogsByCategoryAsync(string catalogType);

        /// <summary>
        /// Obtiene un catálogo específico por ID
        /// </summary>
        Task<CatalogItemDto> GetCatalogByIdAsync(string catalogType, int id);

        /// <summary>
        /// Crea un nuevo registro de catálogo
        /// </summary>
        Task<int> CreateCatalogAsync(string catalogType, CatalogItemDto catalog);

        /// <summary>
        /// Actualiza un registro de catálogo existente
        /// </summary>
        Task<bool> UpdateCatalogAsync(string catalogType, CatalogItemDto catalog);

        /// <summary>
        /// Elimina un registro de catálogo
        /// </summary>
        Task<bool> DeleteCatalogAsync(string catalogType, int id);

        // ── OPERACIONES AVANZADAS

        /// <summary>
        /// Búsqueda global con filtros
        /// </summary>
        Task<List<CatalogItemDto>> SearchCatalogsAsync(string catalogType, string searchTerm,
            bool? isActive = null);

        /// <summary>
        /// Obtiene catálogos activos solamente
        /// </summary>
        Task<List<CatalogItemDto>> GetActiveCatalogsAsync(string catalogType);

        /// <summary>
        /// Desactiva un registro sin eliminarlo (soft delete)
        /// </summary>
        Task<bool> DeactivateCatalogAsync(string catalogType, int id);

        /// <summary>
        /// Validar si un código es único en el catálogo
        /// </summary>
        Task<bool> IsCodeUniqueAsync(string catalogType, string code, int? excludeId = null);

        /// <summary>
        /// Obtiene estadísticas del catálogo
        /// </summary>
        Task<CatalogStatisticsDto> GetCatalogStatisticsAsync(string catalogType);

        // ── OPERACIONES BATCH

        /// <summary>
        /// Importa múltiples registros desde una lista
        /// Transacción atómica
        /// </summary>
        Task<int> BulkImportAsync(string catalogType, List<CatalogItemDto> catalogs);

        /// <summary>
        /// Exporta todos los catálogos de una categoría
        /// </summary>
        Task<List<CatalogItemDto>> ExportAllAsync(string catalogType);
    }

    /// <summary>
    /// DTO para estadísticas de catálogos
    /// </summary>
    public class CatalogStatisticsDto
    {
        public int TotalRecords { get; set; }
        public int ActiveRecords { get; set; }
        public int InactiveRecords { get; set; }
        public DateTime LastUpdated { get; set; }
        public string CatalogType { get; set; }
    }


}
