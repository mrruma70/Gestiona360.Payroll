using Gestiona360.Payroll.Application.Contracts.DTOs;

namespace Gestiona360.Payroll.Application.Contracts.Repositories
{
    /// <summary>
    /// Interfaz del repositorio de catálogos
    /// Abstracción de la capa de datos (Dapper/SQL Server)
    /// </summary>
    public interface ICatalogRepository
    {
        // CRUD básico
        Task<List<CatalogItemDto>> GetAllAsync(string catalogType);
        Task<CatalogItemDto> GetByIdAsync(string catalogType, int id);
        Task<int> InsertAsync(string catalogType, CatalogItemDto catalog);
        Task<bool> UpdateAsync(string catalogType, CatalogItemDto catalog);
        Task<bool> DeleteAsync(string catalogType, int id);

        // Búsqueda y filtrado
        Task<List<CatalogItemDto>> SearchAsync(string catalogType, string searchTerm, bool? isActive = null);
        Task<bool> IsCodeUniqueAsync(string catalogType, string code, int? excludeId = null);

        // Validaciones de lógica de negocio
        Task<int> GetCountAsync(string catalogType, bool? isActive = null);
    }

}
