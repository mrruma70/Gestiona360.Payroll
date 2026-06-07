using Gestiona360.Payroll.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.API.Controllers;

[ApiController]
[Route("api/employees/{id}/documents")]
public class EmployeeDocumentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<EmployeeDocumentsController> _logger;

    public EmployeeDocumentsController(
        ApplicationDbContext context,
        IWebHostEnvironment env,
        ILogger<EmployeeDocumentsController> logger)
    {
        _context = context;
        _env = env;
        _logger = logger;
    }

    [HttpPost("photo")]
    public async Task<IActionResult> UploadPhoto(Guid id, IFormFile file)
    {
        return await UploadDocumentAsync(id, file, "PhotoUrl");
    }

    [HttpPost("id-front")]
    public async Task<IActionResult> UploadIdFront(Guid id, IFormFile file)
    {
        return await UploadDocumentAsync(id, file, "IdFrontUrl");
    }

    [HttpPost("id-back")]
    public async Task<IActionResult> UploadIdBack(Guid id, IFormFile file)
    {
        return await UploadDocumentAsync(id, file, "IdBackUrl");
    }

    private async Task<IActionResult> UploadDocumentAsync(Guid id, IFormFile file, string propertyName)
    {
        try
        {
            _logger.LogInformation($"🔵 [UPLOAD] Iniciando subida para empleado {id}, propiedad: {propertyName}");

            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("🔴 [UPLOAD] Archivo vacío o null");
                return BadRequest("No se seleccionó ningún archivo.");
            }

            _logger.LogInformation($"🔵 [UPLOAD] Archivo recibido: {file.FileName}, Tamaño: {file.Length} bytes");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(ext))
            {
                _logger.LogWarning($"🔴 [UPLOAD] Extensión no permitida: {ext}");
                return BadRequest("Formato no permitido. Solo JPG, PNG o PDF.");
            }

            // ✅ DETERMINAR RUTA CORRECTA
            string webRootPath;

            if (!string.IsNullOrEmpty(_env.WebRootPath))
            {
                webRootPath = _env.WebRootPath;
                _logger.LogInformation($"🟢 [UPLOAD] WebRootPath detectado: {webRootPath}");
            }
            else
            {
                // Fallback: buscar wwwroot en ContentRootPath
                webRootPath = Path.Combine(_env.ContentRootPath, "wwwroot");
                _logger.LogWarning($"🟡 [UPLOAD] WebRootPath es NULL. Usando fallback: {webRootPath}");
            }

            // ✅ CONSTRUIR RUTA COMPLETA
            var uploadsFolder = Path.Combine(webRootPath, "uploads", "employees");

            // ✅ CREAR CARPETA SI NO EXISTE
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
                _logger.LogInformation($"🟢 [UPLOAD] Carpeta creada: {uploadsFolder}");
            }
            else
            {
                _logger.LogInformation($"🟢 [UPLOAD] Carpeta ya existe: {uploadsFolder}");
            }

            // ✅ NOMBRE ÚNICO DEL ARCHIVO
            var fileName = $"{id}_{propertyName}_{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            _logger.LogInformation($"🔵 [UPLOAD] Ruta completa del archivo: {filePath}");

            // ✅ GUARDAR ARCHIVO CON VERIFICACIÓN
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                    await stream.FlushAsync(); // ✅ Asegurar que se escriba todo
                }

                // ✅ VERIFICAR QUE EL ARCHIVO SE GUARDÓ
                if (!System.IO.File.Exists(filePath))
                {
                    _logger.LogError($"🔴 [UPLOAD] ❌ El archivo NO se guardó en: {filePath}");
                    return StatusCode(500, "Error interno: El archivo no se pudo guardar en el servidor.");
                }

                var fileInfo = new FileInfo(filePath);
                _logger.LogInformation($"🟢 [UPLOAD] ✅ Archivo guardado exitosamente. Tamaño en disco: {fileInfo.Length} bytes");
            }
            catch (Exception ex)
            {
                _logger.LogError($"🔴 [UPLOAD] ❌ Error al guardar archivo: {ex.Message}");
                _logger.LogError($"🔴 [UPLOAD] StackTrace: {ex.StackTrace}");
                return StatusCode(500, $"Error al guardar archivo: {ex.Message}");
            }

            // ✅ URL RELATIVA PARA GUARDAR EN BD
            var relativeUrl = $"/uploads/employees/{fileName}";
            _logger.LogInformation($"🔵 [UPLOAD] URL relativa: {relativeUrl}");

            // ✅ ACTUALIZAR BD
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                _logger.LogError($"🔴 [UPLOAD] Empleado {id} no encontrado en BD");
                return NotFound("Empleado no encontrado.");
            }

            var property = typeof(Domain.Entities.Employee).GetProperty(propertyName);
            if (property == null)
            {
                _logger.LogError($"🔴 [UPLOAD] Propiedad '{propertyName}' no existe en la entidad");
                return BadRequest($"La propiedad '{propertyName}' no existe.");
            }

            property.SetValue(employee, relativeUrl);
            employee.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation($"🟢 [UPLOAD] ✅ Proceso completado exitosamente para empleado {id}");

            return Ok(new
            {
                url = relativeUrl,
                message = "Documento subido exitosamente.",
                physicalPath = filePath // ✅ Para debugging (quitar en producción)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"🔴 [UPLOAD] ❌ EXCEPTION GENERAL: {ex.Message}");
            _logger.LogError($"🔴 [UPLOAD] StackTrace: {ex.StackTrace}");
            return StatusCode(500, new
            {
                error = "Error interno al subir archivo",
                detail = ex.Message
            });
        }
    }

    /// <summary>
    /// DELETE: api/employees/{id}/documents/{documentType}
    /// Elimina un documento (foto, cédula frontal o trasera) del empleado y el archivo físico.
    /// </summary>
    [HttpDelete("{documentType}")]
    public async Task<IActionResult> DeleteDocument(Guid id, string documentType)
    {
        try
        {
            _logger.LogInformation($"🔵 [DELETE] Eliminando {documentType} del empleado {id}");

            // 1. Obtener empleado
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                _logger.LogError($"🔴 [DELETE] Empleado {id} no encontrado");
                return NotFound("Empleado no encontrado.");
            }

            // 2. Determinar qué URL borrar según el tipo
            string? urlToDelete = documentType.ToLowerInvariant() switch
            {
                "photo" => employee.PhotoUrl,
                "id-front" => employee.IdFrontUrl,
                "id-back" => employee.IdBackUrl,
                _ => null
            };

            if (string.IsNullOrEmpty(urlToDelete))
            {
                _logger.LogWarning($"🟡 [DELETE] No hay documento de tipo '{documentType}' para este empleado");
                return NotFound($"No hay documento de tipo '{documentType}' para este empleado.");
            }

            _logger.LogInformation($"🔵 [DELETE] URL a eliminar: {urlToDelete}");

            // 3. ✅ CONSTRUIR RUTA FÍSICA CON EL MISMO FALLBACK DEL UPLOAD
            string webRootPath;

            if (!string.IsNullOrEmpty(_env.WebRootPath))
            {
                webRootPath = _env.WebRootPath;
                _logger.LogInformation($"🟢 [DELETE] WebRootPath detectado: {webRootPath}");
            }
            else
            {
                webRootPath = Path.Combine(_env.ContentRootPath, "wwwroot");
                _logger.LogWarning($"🟡 [DELETE] WebRootPath es NULL. Usando fallback: {webRootPath}");
            }

            var relativePath = urlToDelete.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
            var physicalPath = Path.Combine(webRootPath, relativePath);

            _logger.LogInformation($"🔵 [DELETE] Ruta física a eliminar: {physicalPath}");

            // 4. Eliminar archivo si existe
            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
                _logger.LogInformation($"✅ [DELETE] Archivo eliminado: {physicalPath}");
            }
            else
            {
                _logger.LogWarning($"⚠️ [DELETE] Archivo no encontrado en disco: {physicalPath}");
            }

            // 5. Limpiar la propiedad en la base de datos
            switch (documentType.ToLowerInvariant())
            {
                case "photo":
                    employee.PhotoUrl = null;
                    break;
                case "id-front":
                    employee.IdFrontUrl = null;
                    break;
                case "id-back":
                    employee.IdBackUrl = null;
                    break;
            }

            employee.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation($"🟢 [DELETE] ✅ Documento eliminado correctamente para empleado {id}");

            return Ok(new { message = "Documento eliminado correctamente." });
        }
        catch (Exception ex)
        {
            _logger.LogError($"🔴 [DELETE] ❌ EXCEPTION: {ex.Message}");
            _logger.LogError($"🔴 [DELETE] StackTrace: {ex.StackTrace}");
            return StatusCode(500, new
            {
                error = "Error interno al eliminar el documento.",
                detail = ex.Message
            });
        }
    }
}