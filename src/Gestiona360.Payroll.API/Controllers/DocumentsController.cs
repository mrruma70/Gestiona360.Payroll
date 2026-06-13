using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gestiona360.Payroll.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DocumentsController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<DocumentsController> _logger;

        public DocumentsController(IWebHostEnvironment env, ILogger<DocumentsController> logger)
        {
            _env = env;
            _logger = logger;
        }

        /// <summary>
        /// Sube un archivo a una carpeta específica y devuelve la URL pública.
        /// </summary>
        /// <param name="file">Archivo a subir (PDF, JPG, PNG, hasta 5MB)</param>
        /// <param name="folder">Carpeta de destino (por defecto "general")</param>
        /// <returns>Objeto con la URL y el nombre del archivo</returns>
        [HttpPost("upload")]
        [RequestSizeLimit(5 * 1024 * 1024)] // 5 MB
        public async Task<IActionResult> Upload(IFormFile file, [FromQuery] string folder = "general")
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "No se proporcionó un archivo válido." });

            // Validar extensión
            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                return BadRequest(new { error = "Formato no permitido. Use PDF, JPG o PNG." });

            // Determinar ruta de almacenamiento
            var uploadsRoot = string.IsNullOrEmpty(_env.WebRootPath)
                ? Path.Combine(_env.ContentRootPath, "wwwroot")
                : _env.WebRootPath;

            var targetFolder = Path.Combine(uploadsRoot, "uploads", folder);
            if (!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);

            // Generar nombre único
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(targetFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relativeUrl = $"/uploads/{folder}/{fileName}";
            _logger.LogInformation("Documento subido: {Url}", relativeUrl);

            return Ok(new
            {
                url = relativeUrl,
                fileName = file.FileName,
                size = file.Length,
                contentType = file.ContentType // ✅ CORREGIDO: eliminado el espacio en "file"
            });
        }

        /// <summary>
        /// Descarga un archivo desde la carpeta de uploads.
        /// Ruta: GET /api/documents/download/{folder}/{fileName}
        /// </summary>
        [HttpGet("download/{folder}/{fileName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Download(string folder, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName) ||
                fileName.Contains("..") ||
                fileName.Contains("/") ||
                fileName.Contains("\\"))
            {
                return BadRequest(new { error = "Nombre de archivo inválido." });
            }

            var uploadsRoot = string.IsNullOrEmpty(_env.WebRootPath)
                ? Path.Combine(_env.ContentRootPath, "wwwroot")
                : _env.WebRootPath;

            var filePath = Path.Combine(uploadsRoot, "uploads", folder, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                _logger.LogWarning("Archivo no encontrado: {Path}", filePath);
                return NotFound(new { error = "El archivo no existe.", path = filePath });
            }

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            string contentType = extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };

            _logger.LogInformation("Descargando archivo: {Path}", filePath);
            return PhysicalFile(filePath, contentType, fileName);
        }
    }
}
