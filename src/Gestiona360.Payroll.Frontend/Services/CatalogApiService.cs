using System.Net.Http.Json;
using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Application.Contracts.Services;

namespace Gestiona360.Payroll.Frontend.Services
{
    public class CatalogApiService : ICatalogService
    {
        private readonly HttpClient _httpClient;
        public CatalogApiService(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<List<CatalogItemDto>> GetCatalogsByCategoryAsync(string catalogType)
            => await _httpClient.GetFromJsonAsync<List<CatalogItemDto>>($"api/catalogs/{catalogType}") ?? new();

        public async Task<CatalogItemDto> GetCatalogByIdAsync(string catalogType, int id)
            => await _httpClient.GetFromJsonAsync<CatalogItemDto>($"api/catalogs/{catalogType}/{id}");

        public async Task<int> CreateCatalogAsync(string catalogType, CatalogItemDto catalog)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/catalogs/{catalogType}", catalog);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<int>();
        }

        public async Task<bool> UpdateCatalogAsync(string catalogType, CatalogItemDto catalog)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/catalogs/{catalogType}/{catalog.Id}", catalog);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteCatalogAsync(string catalogType, int id)
        {
            var response = await _httpClient.DeleteAsync($"api/catalogs/{catalogType}/{id}");
            return response.IsSuccessStatusCode;
        }

        public Task<List<CatalogItemDto>> SearchCatalogsAsync(string catalogType, string searchTerm, bool? isActive = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<CatalogItemDto>> GetActiveCatalogsAsync(string catalogType)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeactivateCatalogAsync(string catalogType, int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsCodeUniqueAsync(string catalogType, string code, int? excludeId = null)
        {
            throw new NotImplementedException();
        }

        public Task<CatalogStatisticsDto> GetCatalogStatisticsAsync(string catalogType)
        {
            throw new NotImplementedException();
        }

        public Task<int> BulkImportAsync(string catalogType, List<CatalogItemDto> catalogs)
        {
            throw new NotImplementedException();
        }

        public Task<List<CatalogItemDto>> ExportAllAsync(string catalogType)
        {
            throw new NotImplementedException();
        }

        // Implementa el resto de métodos (Search, GetActive, Deactivate, etc.) de forma similar,
        // llamando a endpoints que debes crear en tu API.
    }
}
