using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Application.Contracts.Repositories;
using Microsoft.Extensions.Logging;

namespace Gestiona360.Payroll.Application.Contracts.Services
{
    /// <summary>
    /// Implementación de servicio de catálogos
    /// Lógica de negocio y orquestación con repositorio Dapper
    /// </summary>
    public class CatalogService : ICatalogService
    {
        private readonly ICatalogRepository _catalogRepository;
        private readonly ILogger<CatalogService> _logger;

        public CatalogService(ICatalogRepository catalogRepository, ILogger<CatalogService> logger)
        {
            _catalogRepository = catalogRepository ?? throw new ArgumentNullException(nameof(catalogRepository));
            _logger = logger;
        }

        // ── OPERACIONES BÁSICAS

        public async Task<List<CatalogItemDto>> GetCatalogsByCategoryAsync(string catalogType)
        {
            try
            {
                _logger.LogInformation($"Obteniendo catálogos de tipo: {catalogType}");
                return await _catalogRepository.GetAllAsync(catalogType);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener catálogos: {ex.Message}");
                throw;
            }
        }

        public async Task<CatalogItemDto> GetCatalogByIdAsync(string catalogType, int id)
        {
            return await _catalogRepository.GetByIdAsync(catalogType, id);
        }

        public async Task<int> CreateCatalogAsync(string catalogType, CatalogItemDto catalog)
        {
            // Validación de lógica de negocio
            if (!await IsCodeUniqueAsync(catalogType, catalog.Code))
            {
                throw new InvalidOperationException($"El código '{catalog.Code}' ya existe en este catálogo");
            }

            catalog.CreatedAt = DateTime.UtcNow;
            return await _catalogRepository.InsertAsync(catalogType, catalog);
        }

        public async Task<bool> UpdateCatalogAsync(string catalogType, CatalogItemDto catalog)
        {
            // Validación: asegurar que el código sigue siendo único
            if (!await IsCodeUniqueAsync(catalogType, catalog.Code, catalog.Id))
            {
                throw new InvalidOperationException($"El código '{catalog.Code}' ya existe en otro registro");
            }

            catalog.UpdatedAt = DateTime.UtcNow;
            return await _catalogRepository.UpdateAsync(catalogType, catalog);
        }

        public async Task<bool> DeleteCatalogAsync(string catalogType, int id)
        {
            // Soft delete: marcar como inactivo en lugar de eliminar
            return await _catalogRepository.DeleteAsync(catalogType, id);
        }

        // ── OPERACIONES AVANZADAS

        public async Task<List<CatalogItemDto>> SearchCatalogsAsync(string catalogType, string searchTerm,
            bool? isActive = null)
        {
            return await _catalogRepository.SearchAsync(catalogType, searchTerm, isActive);
        }

        public async Task<List<CatalogItemDto>> GetActiveCatalogsAsync(string catalogType)
        {
            var all = await GetCatalogsByCategoryAsync(catalogType);
            return all.Where(c => c.IsActive).ToList();
        }

        public async Task<bool> DeactivateCatalogAsync(string catalogType, int id)
        {
            var catalog = await GetCatalogByIdAsync(catalogType, id);
            if (catalog == null)
                throw new InvalidOperationException("Catálogo no encontrado");

            catalog.IsActive = false;
            catalog.UpdatedAt = DateTime.UtcNow;
            return await _catalogRepository.UpdateAsync(catalogType, catalog);
        }

        public async Task<bool> IsCodeUniqueAsync(string catalogType, string code, int? excludeId = null)
        {
            return await _catalogRepository.IsCodeUniqueAsync(catalogType, code, excludeId);
        }

        public async Task<CatalogStatisticsDto> GetCatalogStatisticsAsync(string catalogType)
        {
            var catalogs = await GetCatalogsByCategoryAsync(catalogType);

            return new CatalogStatisticsDto
            {
                CatalogType = catalogType,
                TotalRecords = catalogs.Count,
                ActiveRecords = catalogs.Count(c => c.IsActive),
                InactiveRecords = catalogs.Count(c => !c.IsActive),
                LastUpdated = catalogs.Max(c => c.UpdatedAt ?? c.CreatedAt)
            };
        }

        // ── OPERACIONES BATCH

        public async Task<int> BulkImportAsync(string catalogType, List<CatalogItemDto> catalogs)
        {
            var insertedCount = 0;

            foreach (var catalog in catalogs)
            {
                if (await IsCodeUniqueAsync(catalogType, catalog.Code))
                {
                    catalog.CreatedAt = DateTime.UtcNow;
                    await _catalogRepository.InsertAsync(catalogType, catalog);
                    insertedCount++;
                }
            }

            return insertedCount;
        }

        public async Task<List<CatalogItemDto>> ExportAllAsync(string catalogType)
        {
            return await GetCatalogsByCategoryAsync(catalogType);
        }
    }

}
