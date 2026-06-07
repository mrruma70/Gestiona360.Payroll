using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Application.Contracts.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gestiona360.Payroll.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatalogsController : ControllerBase
    {
        private readonly ICatalogService _catalogService;
        public CatalogsController(ICatalogService catalogService) => _catalogService = catalogService;

        [HttpGet("{catalogType}")]
        public async Task<IActionResult> Get(string catalogType) => Ok(await _catalogService.GetCatalogsByCategoryAsync(catalogType));

        [HttpPost("{catalogType}")]
        public async Task<IActionResult> Post(string catalogType, [FromBody] CatalogItemDto dto)
            => Ok(await _catalogService.CreateCatalogAsync(catalogType, dto));

        // Agrega los demás endpoints (GET by id, PUT, DELETE, search, etc.)
    }
}
