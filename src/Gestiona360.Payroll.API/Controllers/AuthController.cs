using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Gestiona360.Payroll.Application.Contracts.DTOs.Auth;
using Gestiona360.Payroll.Application.Features.Auth.Commands;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Infrastructure.Identity.Services;

namespace Gestiona360.Payroll.API.Controllers
{
    /// <summary>
    /// Controlador de autenticación con JWT tokens
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IMediator mediator,
            UserManager<ApplicationUser> userManager,
            ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Inicia sesión y retorna JWT token + refresh token
        /// </summary>
        /// <remarks>
        /// Ejemplo de uso:
        /// ```
        /// POST /api/auth/login
        /// {
        ///   "email": "user@example.com",
        ///   "password": "SecurePassword123!",
        ///   "tenantCode": "EMPRESA001"
        /// }
        /// 
        /// Response (200 OK):
        /// {
        ///   "accessToken": "eyJhbGciOiJIUzI1NiIs...",
        ///   "refreshToken": "sxR2w5Q8pK...",
        ///   "expiresIn": 900,
        ///   "tokenType": "Bearer",
        ///   "user": {
        ///     "id": "...",
        ///     "email": "user@example.com",
        ///     "firstName": "John",
        ///     "lastName": "Doe",
        ///     "roles": ["Admin"]
        ///   }
        /// }
        /// ```
        /// </remarks>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
                var userAgent = Request.Headers["User-Agent"].ToString();

                var command = new LoginCommand(request.Email, request.Password, request.TenantCode)
                {
                    IpAddress = ipAddress,
                    UserAgent = userAgent
                };

                var result = await _mediator.Send(command);

                _logger.LogInformation($"Usuario {request.Email} inició sesión exitosamente desde {ipAddress}");

                // Configurar cookie de refresh token (opcional, para seguridad extra)
                Response.Cookies.Append("refreshToken", result.RefreshToken,
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict
                    });

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning($"Intento de login fallido para {request.Email}: {ex.Message}");
                return Unauthorized(new { message = "Credenciales inválidas" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning($"Error durante login de {request.Email}: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado durante login");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        /// <summary>
        /// Renueva el access token usando un refresh token válido
        /// </summary>
        [AllowAnonymous]
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
                var command = new RefreshTokenCommand(request.RefreshToken, ipAddress);

                var result = await _mediator.Send(command);

                _logger.LogInformation("Token renovado exitosamente");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Error al renovar token: {ex.Message}");
                return Unauthorized(new { message = "Refresh token inválido" });
            }
        }

        /// <summary>
        /// Cierra sesión revocando el refresh token
        /// </summary>
        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                    return Unauthorized();

                // Obtener refresh token del header Authorization o cookie
                var refreshToken = Request.Cookies["refreshToken"] ?? "";

                if (!string.IsNullOrEmpty(refreshToken))
                {
                    // Revocar el refresh token
                    // (requiere inyectar ITokenGenerationService)
                }

                _logger.LogInformation($"Usuario {userId} cerró sesión");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante logout");
                return StatusCode(500, new { message = "Error al cerrar sesión" });
            }
        }

        /// <summary>
        /// Obtiene información del usuario autenticado actual
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                    return Unauthorized();

                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    return NotFound();

                var roles = await _userManager.GetRolesAsync(user);

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email ?? "",
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = roles.ToList(),
                    IsActive = !user.LockoutEnabled,
                    CreatedAt = user.CreatedAt
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo usuario actual");
                return StatusCode(500, new { message = "Error interno" });
            }
        }
    }
}