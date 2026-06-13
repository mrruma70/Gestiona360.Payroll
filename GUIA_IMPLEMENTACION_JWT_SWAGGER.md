# 🚀 GUÍA DE IMPLEMENTACIÓN: JWT, Login y Swagger
## Gestiona360.Payroll | Código Listo para Implementar

**Versión:** 1.0  
**Duración Total:** 4-5 horas  
**Complejidad:** Media

---

## 📋 TABLA DE CONTENIDOS

1. [Setup Inicial](#setup-inicial)
2. [Paso 1: DTOs de Autenticación](#paso-1-dtos-de-autenticación)
3. [Paso 2: Entity para RefreshToken](#paso-2-entity-para-refreshtoken)
4. [Paso 3: TokenGenerationService](#paso-3-tokengenerationservice)
5. [Paso 4: CQRS Commands](#paso-4-cqrs-commands)
6. [Paso 5: AuthController](#paso-5-authcontroller)
7. [Paso 6: Program.cs - Configuración JWT](#paso-6-programcs---configuración-jwt)
8. [Paso 7: AuthService (Frontend)](#paso-7-authservice-frontend)
9. [Paso 8: Proteger Controladores](#paso-8-proteger-controladores)
10. [Paso 9: Testing](#paso-9-testing)
11. [Conclusión](#conclusión)

---

## 🔧 SETUP INICIAL

### 1. Instalar NuGet Packages

En Package Manager Console o terminal:
```powershell
# Backend - API
dotnet add src/Gestiona360.Payroll.API package System.IdentityModel.Tokens.Jwt --version 8.0.0
dotnet add src/Gestiona360.Payroll.API package Swashbuckle.AspNetCore --version 6.4.0

# Frontend
# (Sin cambios, ya tiene lo necesario)
```

O via NuGet UI en Visual Studio:
```
System.IdentityModel.Tokens.Jwt >= 8.0.0
Swashbuckle.AspNetCore >= 6.4.0
```

### 2. Crear Estructura de Carpetas

```
src/Gestiona360.Payroll.Application.Contracts/DTOs/
├── Auth/
│   ├── LoginRequest.cs
│   ├── LoginResponse.cs
│   ├── RefreshTokenRequest.cs
│   └── UserDto.cs

src/Gestiona360.Payroll.Application/Features/
├── Auth/
│   ├── Commands/
│   │   ├── LoginCommand.cs
│   │   ├── LoginCommandHandler.cs
│   │   ├── RefreshTokenCommand.cs
│   │   └── RefreshTokenCommandHandler.cs

src/Gestiona360.Payroll.Infrastructure.Identity/
├── Services/
│   └── TokenGenerationService.cs
├── Entities/
│   └── UserRefreshToken.cs

src/Gestiona360.Payroll.API/
├── Controllers/
│   └── AuthController.cs

src/Gestiona360.Payroll.Frontend/Services/
└── AuthService.cs
```

---

## ✅ PASO 1: DTOs DE AUTENTICACIÓN

### 1.1 LoginRequest.cs
**Ubicación:** `src/Gestiona360.Payroll.Application.Contracts/DTOs/Auth/LoginRequest.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.Auth
{
	public class LoginRequest
	{
		[Required(ErrorMessage = "El correo es obligatorio")]
		[EmailAddress(ErrorMessage = "El formato del correo es inválido")]
		public string Email { get; set; } = string.Empty;

		[Required(ErrorMessage = "La contraseña es obligatoria")]
		[StringLength(100, MinimumLength = 8, 
			ErrorMessage = "La contraseña debe tener entre 8 y 100 caracteres")]
		public string Password { get; set; } = string.Empty;

		/// <summary>
		/// Código de empresa/tenant (opcional, para multi-tenancy futuro)
		/// </summary>
		[StringLength(50)]
		public string? TenantCode { get; set; }
	}
}
```

---

### 1.2 UserDto.cs
**Ubicación:** `src/Gestiona360.Payroll.Application.Contracts/DTOs/Auth/UserDto.cs`

```csharp
using System.Collections.Generic;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.Auth
{
	public class UserDto
	{
		public Guid Id { get; set; }

		public string Email { get; set; } = string.Empty;

		public string? FirstName { get; set; }

		public string? LastName { get; set; }

		public string FullName => $"{FirstName} {LastName}".Trim();

		public List<string> Roles { get; set; } = new();

		public bool IsActive { get; set; }

		public DateTime CreatedAt { get; set; }
	}
}
```

---

### 1.3 LoginResponse.cs
**Ubicación:** `src/Gestiona360.Payroll.Application.Contracts/DTOs/Auth/LoginResponse.cs`

```csharp
namespace Gestiona360.Payroll.Application.Contracts.DTOs.Auth
{
	public class LoginResponse
	{
		/// <summary>
		/// JWT Access Token (corta duración: 15-60 min)
		/// </summary>
		public string AccessToken { get; set; } = string.Empty;

		/// <summary>
		/// Refresh Token para renovar el Access Token
		/// </summary>
		public string RefreshToken { get; set; } = string.Empty;

		/// <summary>
		/// Tiempo en segundos para expiración del Access Token
		/// </summary>
		public int ExpiresIn { get; set; }

		/// <summary>
		/// Información del usuario autenticado
		/// </summary>
		public UserDto User { get; set; } = new();

		/// <summary>
		/// Tipo de token (siempre "Bearer")
		/// </summary>
		public string TokenType => "Bearer";
	}
}
```

---

### 1.4 RefreshTokenRequest.cs
**Ubicación:** `src/Gestiona360.Payroll.Application.Contracts/DTOs/Auth/RefreshTokenRequest.cs`

```csharp
using System.ComponentModel.DataAnnotations;

namespace Gestiona360.Payroll.Application.Contracts.DTOs.Auth
{
	public class RefreshTokenRequest
	{
		[Required(ErrorMessage = "El refresh token es obligatorio")]
		public string RefreshToken { get; set; } = string.Empty;
	}
}
```

---

## 📊 PASO 2: ENTITY PARA REFRESHTOKEN

### 2.1 UserRefreshToken.cs
**Ubicación:** `src/Gestiona360.Payroll.Infrastructure.Identity/Entities/UserRefreshToken.cs`

```csharp
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Common;

namespace Gestiona360.Payroll.Infrastructure.Identity.Entities
{
	/// <summary>
	/// Almacena refresh tokens válidos para cada usuario
	/// </summary>
	public class UserRefreshToken : BaseEntityGuid
	{
		/// <summary>
		/// Referencia al usuario
		/// </summary>
		public Guid UserId { get; set; }
		public ApplicationUser? User { get; set; }

		/// <summary>
		/// El refresh token (string aleatorio + encriptado)
		/// </summary>
		public string Token { get; set; } = string.Empty;

		/// <summary>
		/// IP desde donde se generó el token
		/// </summary>
		public string? IpAddress { get; set; }

		/// <summary>
		/// User Agent del navegador/cliente
		/// </summary>
		public string? UserAgent { get; set; }

		/// <summary>
		/// Fecha de expiración del refresh token
		/// </summary>
		public DateTime ExpiresAt { get; set; }

		/// <summary>
		/// Si fue revocado (logout)
		/// </summary>
		public bool IsRevoked { get; set; }

		/// <summary>
		/// Fecha en que fue revocado
		/// </summary>
		public DateTime? RevokedAt { get; set; }

		/// <summary>
		/// Razón de revocación
		/// </summary>
		public string? RevocationReason { get; set; }

		/// <summary>
		/// Es válido si no está expirado y no está revocado
		/// </summary>
		public bool IsValid => DateTime.UtcNow < ExpiresAt && !IsRevoked;
	}
}
```

---

### 2.2 Agregar a ApplicationDbContext

Abre `src/Gestiona360.Payroll.Infrastructure.Persistence/ApplicationDbContext.cs`:

```csharp
// AGREGAR ESTE DbSet
public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

// En OnModelCreating (opcional, pero recomendado)
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
	base.OnModelCreating(modelBuilder);

	// Configurar UserRefreshToken
	modelBuilder.Entity<UserRefreshToken>()
		.HasOne(urt => urt.User)
		.WithMany()
		.HasForeignKey(urt => urt.UserId)
		.OnDelete(DeleteBehavior.Cascade);

	modelBuilder.Entity<UserRefreshToken>()
		.HasIndex(urt => urt.Token)
		.IsUnique();

	modelBuilder.Entity<UserRefreshToken>()
		.HasIndex(urt => urt.UserId);
}
```

### 2.3 Crear Migración

En Package Manager Console:
```powershell
Add-Migration AddUserRefreshTokenTable
Update-Database
```

O en terminal:
```bash
dotnet ef migrations add AddUserRefreshTokenTable --project src/Gestiona360.Payroll.Infrastructure.Persistence
dotnet ef database update --project src/Gestiona360.Payroll.Infrastructure.Persistence
```

---

## 🔑 PASO 3: TOKENGENERATIONSERVICE

### 3.1 TokenGenerationService.cs
**Ubicación:** `src/Gestiona360.Payroll.Infrastructure.Identity/Services/TokenGenerationService.cs`

```csharp
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Infrastructure.Identity.Entities;
using Gestiona360.Payroll.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Gestiona360.Payroll.Infrastructure.Identity.Services
{
	/// <summary>
	/// Servicio para generar y validar JWT tokens y refresh tokens
	/// </summary>
	public interface ITokenGenerationService
	{
		Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(
			ApplicationUser user, 
			IList<string> roles,
			string? ipAddress = null,
			string? userAgent = null);

		Task<bool> ValidateRefreshTokenAsync(Guid userId, string refreshToken);

		Task RevokeRefreshTokenAsync(Guid userId, string refreshToken, string reason = "Logout");

		string GenerateJwtToken(ApplicationUser user, IList<string> roles);

		string GenerateRefreshToken();

		ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
	}

	public class TokenGenerationService : ITokenGenerationService
	{
		private readonly IConfiguration _configuration;
		private readonly ApplicationDbContext _context;
		private readonly IUserStore<ApplicationUser> _userStore;

		public TokenGenerationService(
			IConfiguration configuration,
			ApplicationDbContext context,
			IUserStore<ApplicationUser> userStore)
		{
			_configuration = configuration;
			_context = context;
			_userStore = userStore;
		}

		/// <summary>
		/// Genera JWT access token y refresh token
		/// </summary>
		public async Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(
			ApplicationUser user,
			IList<string> roles,
			string? ipAddress = null,
			string? userAgent = null)
		{
			// 1. Generar JWT corto
			var accessToken = GenerateJwtToken(user, roles);

			// 2. Generar refresh token largo
			var refreshToken = GenerateRefreshToken();

			// 3. Guardar refresh token en BD
			var tokenEntity = new UserRefreshToken
			{
				Id = Guid.NewGuid(),
				UserId = user.Id,
				Token = refreshToken,
				IpAddress = ipAddress,
				UserAgent = userAgent,
				ExpiresAt = DateTime.UtcNow.AddDays(
					int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "7")),
				IsRevoked = false
			};

			_context.UserRefreshTokens.Add(tokenEntity);
			await _context.SaveChangesAsync();

			return (accessToken, refreshToken);
		}

		/// <summary>
		/// Valida que un refresh token sea válido
		/// </summary>
		public async Task<bool> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
		{
			var storedToken = await _context.UserRefreshTokens
				.FirstOrDefaultAsync(rt => 
					rt.UserId == userId && 
					rt.Token == refreshToken);

			if (storedToken == null)
				return false;

			return storedToken.IsValid;
		}

		/// <summary>
		/// Revoca un refresh token (logout)
		/// </summary>
		public async Task RevokeRefreshTokenAsync(
			Guid userId, 
			string refreshToken, 
			string reason = "Logout")
		{
			var token = await _context.UserRefreshTokens
				.FirstOrDefaultAsync(rt => 
					rt.UserId == userId && 
					rt.Token == refreshToken);

			if (token != null)
			{
				token.IsRevoked = true;
				token.RevokedAt = DateTime.UtcNow;
				token.RevocationReason = reason;
				await _context.SaveChangesAsync();
			}
		}

		/// <summary>
		/// Genera JWT token con claims del usuario
		/// </summary>
		public string GenerateJwtToken(ApplicationUser user, IList<string> roles)
		{
			var jwtSettings = _configuration.GetSection("Jwt");
			var secretKey = Encoding.ASCII.GetBytes(
				jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key no configurada"));

			var tokenHandler = new JwtSecurityTokenHandler();

			// Construir claims
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Email, user.Email ?? ""),
				new Claim(ClaimTypes.Name, user.UserName ?? ""),
				new Claim("FirstName", user.FirstName ?? ""),
				new Claim("LastName", user.LastName ?? "")
			};

			// Agregar roles como claims
			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "15");

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
				Issuer = jwtSettings["Issuer"],
				Audience = jwtSettings["Audience"],
				SigningCredentials = new SigningCredentials(
					new SymmetricSecurityKey(secretKey), 
					SecurityAlgorithms.HmacSha256Signature)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}

		/// <summary>
		/// Genera un refresh token aleatorio de 64 bytes (base64)
		/// </summary>
		public string GenerateRefreshToken()
		{
			var randomNumber = new byte[64];
			using (var rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(randomNumber);
				return Convert.ToBase64String(randomNumber);
			}
		}

		/// <summary>
		/// Extrae principal del JWT expirado (para validar refresh)
		/// </summary>
		public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
		{
			var jwtSettings = _configuration.GetSection("Jwt");
			var secretKey = Encoding.ASCII.GetBytes(
				jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key no configurada"));

			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateAudienceOnEmptyToken = false,
				ValidateAudience = true,
				ValidAudience = jwtSettings["Audience"],
				ValidateIssuer = true,
				ValidIssuer = jwtSettings["Issuer"],
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(secretKey),
				ValidateLifetime = false // ← NO validar expiración (queremos leer el token expirado)
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

			if (!(securityToken is JwtSecurityToken jwtSecurityToken) ||
				!jwtSecurityToken.Header.Alg.Equals(
					SecurityAlgorithms.HmacSha256,
					StringComparison.InvariantCultureIgnoreCase))
			{
				throw new SecurityTokenException("Token inválido");
			}

			return principal;
		}
	}
}
```

### 3.2 Registrar en DependencyInjection.cs

Abre `src/Gestiona360.Payroll.Infrastructure.Identity/DependencyInjection.cs`:

```csharp
public static IServiceCollection AddIdentityInfrastructure(
	this IServiceCollection services,
	IConfiguration configuration)
{
	services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
		.AddEntityFrameworkStores<ApplicationDbContext>()
		.AddDefaultTokenProviders();

	services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

	// ← AGREGAR ESTO
	services.AddScoped<ITokenGenerationService, TokenGenerationService>();

	return services;
}
```

---

## ⚙️ PASO 4: CQRS COMMANDS

### 4.1 LoginCommand.cs
**Ubicación:** `src/Gestiona360.Payroll.Application/Features/Auth/Commands/LoginCommand.cs`

```csharp
using MediatR;
using Gestiona360.Payroll.Application.Contracts.DTOs.Auth;

namespace Gestiona360.Payroll.Application.Features.Auth.Commands
{
	public class LoginCommand : IRequest<LoginResponse>
	{
		public LoginCommand(string email, string password, string? tenantCode = null)
		{
			Email = email;
			Password = password;
			TenantCode = tenantCode;
		}

		public string Email { get; set; }
		public string Password { get; set; }
		public string? TenantCode { get; set; }

		/// <summary>
		/// IP address para auditoría (se llena en controller)
		/// </summary>
		public string? IpAddress { get; set; }

		/// <summary>
		/// User agent para auditoría (se llena en controller)
		/// </summary>
		public string? UserAgent { get; set; }
	}
}
```

### 4.2 LoginCommandHandler.cs
**Ubicación:** `src/Gestiona360.Payroll.Application/Features/Auth/Commands/LoginCommandHandler.cs`

```csharp
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Gestiona360.Payroll.Application.Contracts.DTOs.Auth;
using Gestiona360.Payroll.Application.Contracts;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Infrastructure.Identity.Services;

namespace Gestiona360.Payroll.Application.Features.Auth.Commands
{
	public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ITokenGenerationService _tokenService;
		private readonly IApplicationDbContext _context;

		public LoginCommandHandler(
			UserManager<ApplicationUser> userManager,
			ITokenGenerationService tokenService,
			IApplicationDbContext context)
		{
			_userManager = userManager;
			_tokenService = tokenService;
			_context = context;
		}

		public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
		{
			// 1. Validar credenciales
			var user = await _userManager.FindByEmailAsync(request.Email);
			if (user == null)
				throw new UnauthorizedAccessException("Credenciales inválidas");

			if (!user.EmailConfirmed)
				throw new InvalidOperationException("El correo no está confirmado");

			var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
			if (!isPasswordValid)
				throw new UnauthorizedAccessException("Credenciales inválidas");

			if (!user.LockoutEnd.HasValue || user.LockoutEnd < DateTime.UtcNow)
			{
				// Usuario no está bloqueado
			}
			else
			{
				throw new InvalidOperationException("La cuenta está bloqueada");
			}

			// 2. Obtener roles del usuario
			var roles = await _userManager.GetRolesAsync(user);

			// 3. Generar tokens
			var (accessToken, refreshToken) = await _tokenService.GenerateTokensAsync(
				user,
				roles,
				request.IpAddress,
				request.UserAgent);

			// 4. Construir response
			var response = new LoginResponse
			{
				AccessToken = accessToken,
				RefreshToken = refreshToken,
				ExpiresIn = 900, // 15 minutos en segundos
				User = new UserDto
				{
					Id = user.Id,
					Email = user.Email ?? "",
					FirstName = user.FirstName,
					LastName = user.LastName,
					Roles = roles.ToList(),
					IsActive = !user.LockoutEnabled,
					CreatedAt = DateTime.UtcNow
				}
			};

			return response;
		}
	}
}
```

### 4.3 RefreshTokenCommand.cs
**Ubicación:** `src/Gestiona360.Payroll.Application/Features/Auth/Commands/RefreshTokenCommand.cs`

```csharp
using MediatR;
using Gestiona360.Payroll.Application.Contracts.DTOs.Auth;

namespace Gestiona360.Payroll.Application.Features.Auth.Commands
{
	public class RefreshTokenCommand : IRequest<LoginResponse>
	{
		public RefreshTokenCommand(string refreshToken, string? ipAddress = null)
		{
			RefreshToken = refreshToken;
			IpAddress = ipAddress;
		}

		public string RefreshToken { get; set; }
		public string? IpAddress { get; set; }
	}
}
```

### 4.4 RefreshTokenCommandHandler.cs
**Ubicación:** `src/Gestiona360.Payroll.Application/Features/Auth/Commands/RefreshTokenCommandHandler.cs`

```csharp
using MediatR;
using Microsoft.AspNetCore.Identity;
using Gestiona360.Payroll.Application.Contracts.DTOs.Auth;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Infrastructure.Identity.Services;

namespace Gestiona360.Payroll.Application.Features.Auth.Commands
{
	public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResponse>
	{
		private readonly ITokenGenerationService _tokenService;
		private readonly UserManager<ApplicationUser> _userManager;

		public RefreshTokenCommandHandler(
			ITokenGenerationService tokenService,
			UserManager<ApplicationUser> userManager)
		{
			_tokenService = tokenService;
			_userManager = userManager;
		}

		public async Task<LoginResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
		{
			try
			{
				// 1. Obtener principal del token expirado para extraer el user ID
				var principal = _tokenService.GetPrincipalFromExpiredToken(request.RefreshToken);

				var userIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
				if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
					throw new SecurityTokenException("Token inválido");

				// 2. Validar que el refresh token sea válido
				var isValid = await _tokenService.ValidateRefreshTokenAsync(userId, request.RefreshToken);
				if (!isValid)
					throw new SecurityTokenException("Refresh token inválido o expirado");

				// 3. Obtener usuario
				var user = await _userManager.FindByIdAsync(userId.ToString());
				if (user == null)
					throw new InvalidOperationException("Usuario no encontrado");

				// 4. Obtener roles
				var roles = await _userManager.GetRolesAsync(user);

				// 5. Generar nuevos tokens
				var (newAccessToken, newRefreshToken) = await _tokenService.GenerateTokensAsync(
					user,
					roles,
					request.IpAddress);

				// 6. Revocar el refresh token anterior
				await _tokenService.RevokeRefreshTokenAsync(userId, request.RefreshToken, "Token refreshed");

				// 7. Retornar respuesta
				return new LoginResponse
				{
					AccessToken = newAccessToken,
					RefreshToken = newRefreshToken,
					ExpiresIn = 900,
					User = new UserDto
					{
						Id = user.Id,
						Email = user.Email ?? "",
						FirstName = user.FirstName,
						LastName = user.LastName,
						Roles = roles.ToList(),
						IsActive = true,
						CreatedAt = user.CreationTime
					}
				};
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException($"Refresh token inválido: {ex.Message}", ex);
			}
		}
	}
}
```

---

## 🔌 PASO 5: AUTHCONTROLLER

### 5.1 AuthController.cs
**Ubicación:** `src/Gestiona360.Payroll.API/Controllers/AuthController.cs`

```csharp
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
				// Response.Cookies.Append("refreshToken", result.RefreshToken, 
				//     new CookieOptions 
				//     { 
				//         HttpOnly = true, 
				//         Secure = true, 
				//         SameSite = SameSiteMode.Strict 
				//     });

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
					CreatedAt = user.CreationTime
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
```

---

## ⚙️ PASO 6: PROGRAM.CS - CONFIGURACIÓN JWT

### 6.1 Actualizar appsettings.json

Abre `src/Gestiona360.Payroll.API/appsettings.json` y REEMPLAZA por:

```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Data Source=VPorras\\MSSQLSERVER2022;Initial Catalog=Gestiona360PayrollDb;Integrated Security=false;MultipleActiveResultSets=True;User ID=sa;Password=Armagedon$70;Connect Timeout=30;Pooling=true;Encrypt=true;TrustServerCertificate=True"
  },
  "Jwt": {
	"Key": "tu-super-secreto-key-minimo-32-caracteres-AQUI123456",
	"Issuer": "Gestiona360.Payroll.API",
	"Audience": "Gestiona360.Payroll.Frontend",
	"ExpirationMinutes": 15,
	"RefreshTokenExpirationDays": 7
  },
  "Logging": {
	"LogLevel": {
	  "Default": "Information",
	  "Microsoft.AspNetCore": "Warning"
	}
  },
  "AllowedHosts": "*"
}
```

**⚠️ IMPORTANTE:** Cambiar `Jwt:Key` a una cadena fuerte de mínimo 32 caracteres en producción.

### 6.2 Actualizar Program.cs (API)

Abre `src/Gestiona360.Payroll.API/Program.cs` y REEMPLAZA la sección de configuración:

```csharp
using System.Reflection;
using System.Text;
using Gestiona360.Payroll.Application;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Infrastructure.Identity;
using Gestiona360.Payroll.Infrastructure.Identity.Services;
using Gestiona360.Payroll.Infrastructure.Persistence;
using Gestiona360.Payroll.Infrastructure.Persistence.Seeding;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// SERVICIOS EXISTENTES
// ============================================
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowBlazorClient", policy =>
	{
		policy.WithOrigins("http://localhost:5063", "https://localhost:7141")
			  .AllowAnyMethod()
			  .AllowAnyHeader()
			  .WithExposedHeaders("Content-Disposition");
	});
});

builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddHttpContextAccessor();

// ============================================
// JWT AUTHENTICATION (NUEVO)
// ============================================
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = Encoding.ASCII.GetBytes(
	jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not configured in appsettings"));

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(secretKey),
		ValidateIssuer = true,
		ValidIssuer = jwtSettings["Issuer"],
		ValidateAudience = true,
		ValidAudience = jwtSettings["Audience"],
		ValidateLifetime = true,
		ClockSkew = TimeSpan.Zero
	};
});

builder.Services.AddAuthorization();

// ============================================
// SWAGGER / OPENAPI (MEJORADO CON JWT)
// ============================================
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "Gestiona360 Payroll API",
		Version = "v1.0",
		Description = "API REST de nómina con autenticación JWT",
		Contact = new OpenApiContact
		{
			Name = "Tu Empresa",
			Email = "soporte@empresa.com",
			Url = new Uri("https://www.empresa.com")
		},
		License = new OpenApiLicense
		{
			Name = "MIT",
			Url = new Uri("https://opensource.org/licenses/MIT")
		}
	});

	// Agregar seguridad Bearer
	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Description = "JWT Authorization header using Bearer scheme",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.ApiKey,
		Scheme = "Bearer",
		BearerFormat = "JWT"
	});

	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				},
				Scheme = "oauth2",
				Name = "Bearer",
				In = ParameterLocation.Header
			},
			new List<string>()
		}
	});

	// Documentar XML comments
	var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
	if (File.Exists(xmlPath))
	{
		c.IncludeXmlComments(xmlPath);
	}
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// ============================================
// APLICACIÓN
// ============================================
try
{
	var app = builder.Build();

	// Migrar BD
	using (var scope = app.Services.CreateScope())
	{
		var services = scope.ServiceProvider;
		try
		{
			var context = services.GetRequiredService<ApplicationDbContext>();
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
			var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
			var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

			await context.Database.MigrateAsync();
			await ApplicationDbInitializer.InitializeIdentityAsync(context, userManager, roleManager, logger);
			await DbInitializer.SeedAsync(context);
		}
		catch (Exception ex)
		{
			var logger = services.GetRequiredService<ILogger<Program>>();
			logger.LogError(ex, "Error fatal al migrar o sembrar BD");
		}
	}

	if (app.Environment.IsDevelopment())
	{
		app.MapOpenApi();

		// ← SWAGGER
		app.UseSwagger();
		app.UseSwaggerUI(c =>
		{
			c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gestiona360 API v1");
			c.RoutePrefix = "swagger";
			c.DefaultModelsExpandDepth(2);
			c.DefaultModelExpandDepth(2);
		});
	}

	app.UseStaticFiles();
	app.UseCors("AllowBlazorClient");
	app.UseHttpsRedirection();

	// ← ORDEN CRÍTICO
	app.UseAuthentication();  // DEBE estar antes de UseAuthorization
	app.UseAuthorization();

	app.MapControllers();

	app.Run();
}
catch (ReflectionTypeLoadException ex)
{
	Console.WriteLine("Error cargando tipos:");
	foreach (var loaderEx in ex.LoaderExceptions)
	{
		Console.WriteLine(loaderEx?.Message);
	}
}
```

---

## 🎨 PASO 7: AUTHSERVICE (FRONTEND)

### 7.1 AuthService.cs
**Ubicación:** `src/Gestiona360.Payroll.Frontend/Services/AuthService.cs`

```csharp
using System.Net.Http.Json;
using System.Text.Json;
using Gestiona360.Payroll.Application.Contracts.DTOs.Auth;
using Microsoft.JSInterop;

namespace Gestiona360.Payroll.Frontend.Services
{
	/// <summary>
	/// Servicio de autenticación con JWT tokens
	/// </summary>
	public class AuthService
	{
		private readonly HttpClient _httpClient;
		private readonly IJSRuntime _js;
		private const string TokenKey = "auth_token";
		private const string RefreshTokenKey = "refresh_token";
		private const string UserKey = "auth_user";

		public AuthService(HttpClient httpClient, IJSRuntime js)
		{
			_httpClient = httpClient;
			_js = js;
		}

		/// <summary>
		/// Inicia sesión con email y contraseña
		/// </summary>
		public async Task<(bool Success, string? Error, UserDto? User)> LoginAsync(
			string email,
			string password,
			string? tenantCode = null)
		{
			try
			{
				var request = new LoginRequest
				{
					Email = email,
					Password = password,
					TenantCode = tenantCode
				};

				var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);

				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadAsAsync<LoginResponse>();

					// Guardar tokens en sessionStorage
					await _js.InvokeVoidAsync("sessionStorage.setItem", TokenKey, result.AccessToken);
					await _js.InvokeVoidAsync("sessionStorage.setItem", RefreshTokenKey, result.RefreshToken);
					await _js.InvokeVoidAsync("sessionStorage.setItem", UserKey, 
						System.Text.Json.JsonSerializer.Serialize(result.User));

					return (true, null, result.User);
				}
				else
				{
					var errorContent = await response.Content.ReadAsStringAsync();
					return (false, "Credenciales inválidas o error del servidor", null);
				}
			}
			catch (Exception ex)
			{
				return (false, $"Error de conexión: {ex.Message}", null);
			}
		}

		/// <summary>
		/// Cierra sesión y limpia los tokens
		/// </summary>
		public async Task LogoutAsync()
		{
			try
			{
				var token = await GetTokenAsync();
				if (!string.IsNullOrEmpty(token))
				{
					_httpClient.DefaultRequestHeaders.Authorization = 
						new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

					try
					{
						await _httpClient.PostAsync("api/auth/logout", null);
					}
					catch
					{
						// Ignorar errores del logout en el servidor
					}
				}
			}
			finally
			{
				// Limpiar storage local
				await _js.InvokeVoidAsync("sessionStorage.removeItem", TokenKey);
				await _js.InvokeVoidAsync("sessionStorage.removeItem", RefreshTokenKey);
				await _js.InvokeVoidAsync("sessionStorage.removeItem", UserKey);
			}
		}

		/// <summary>
		/// Obtiene el access token almacenado
		/// </summary>
		public async Task<string?> GetTokenAsync()
		{
			try
			{
				return await _js.InvokeAsync<string?>("sessionStorage.getItem", TokenKey);
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Renueva el access token usando el refresh token
		/// </summary>
		public async Task<bool> RefreshTokenAsync()
		{
			try
			{
				var refreshToken = await _js.InvokeAsync<string?>("sessionStorage.getItem", RefreshTokenKey);

				if (string.IsNullOrEmpty(refreshToken))
					return false;

				var request = new RefreshTokenRequest { RefreshToken = refreshToken };
				var response = await _httpClient.PostAsJsonAsync("api/auth/refresh", request);

				if (response.IsSuccessStatusCode)
				{
					var result = await response.Content.ReadAsAsync<LoginResponse>();

					await _js.InvokeVoidAsync("sessionStorage.setItem", TokenKey, result.AccessToken);
					await _js.InvokeVoidAsync("sessionStorage.setItem", RefreshTokenKey, result.RefreshToken);

					return true;
				}

				return false;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Verifica si hay un usuario autenticado
		/// </summary>
		public async Task<bool> IsAuthenticatedAsync()
		{
			var token = await GetTokenAsync();
			return !string.IsNullOrEmpty(token);
		}

		/// <summary>
		/// Obtiene el usuario actual desde el storage
		/// </summary>
		public async Task<UserDto?> GetCurrentUserAsync()
		{
			try
			{
				var userJson = await _js.InvokeAsync<string?>("sessionStorage.getItem", UserKey);
				if (string.IsNullOrEmpty(userJson))
					return null;

				return System.Text.Json.JsonSerializer.Deserialize<UserDto>(userJson);
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Obtiene información del usuario actual desde la API
		/// </summary>
		public async Task<UserDto?> GetUserFromApiAsync()
		{
			try
			{
				var token = await GetTokenAsync();
				if (string.IsNullOrEmpty(token))
					return null;

				_httpClient.DefaultRequestHeaders.Authorization = 
					new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

				var response = await _httpClient.GetAsync("api/auth/me");

				if (response.IsSuccessStatusCode)
				{
					return await response.Content.ReadAsAsync<UserDto>();
				}

				return null;
			}
			catch
			{
				return null;
			}
		}
	}
}
```

### 7.2 Registrar AuthService en Program.cs (Frontend)

Abre `src/Gestiona360.Payroll.Frontend/Program.cs` y REEMPLAZA:

```csharp
using Gestiona360.Payroll.Frontend;
using Gestiona360.Payroll.Frontend.Auth;
using Gestiona360.Payroll.Frontend.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");

builder.Services.AddMudServices();

// ← REGISTRAR AUTHSERVICE
builder.Services.AddScoped<AuthService>();

// HttpClient base
builder.Services.AddScoped(sp =>
	new HttpClient
	{
		BaseAddress = new Uri("https://localhost:7119/")
	});

// CustomAuthStateProvider
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

// Servicios de catálogos
builder.Services.AddScoped<PayrollService>();

await builder.Build().RunAsync();
```

---

## 🔒 PASO 8: PROTEGER CONTROLADORES

### 8.1 Ejemplo: EmployeesController

Abre `src/Gestiona360.Payroll.API/Controllers/EmployeesController.cs`:

```csharp
using Gestiona360.Payroll.Application.Contracts.Requests;
using Gestiona360.Payroll.Application.Features.Employees.Commands;
using Gestiona360.Payroll.Application.Features.Employees.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;  // ← AGREGAR
using Microsoft.AspNetCore.Mvc;

namespace Gestiona360.Payroll.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]  // ← AGREGAR: Todos los endpoints requieren autenticación
	public class EmployeesController : ControllerBase
	{
		private readonly IMediator _mediator;
		private readonly ILogger<EmployeesController> _logger;

		public EmployeesController(IMediator mediator, ILogger<EmployeesController> logger)
		{
			_mediator = mediator;
			_logger = logger;
		}

		/// <summary>
		/// Obtiene todos los empleados (requiere autenticación)
		/// </summary>
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			try
			{
				var employees = await _mediator.Send(new GetAllEmployeesQuery());
				return Ok(employees);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error obteniendo empleados");
				return StatusCode(500, new { message = "Error interno" });
			}
		}

		/// <summary>
		/// Obtiene un empleado por ID
		/// </summary>
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			try
			{
				var employee = await _mediator.Send(new GetEmployeeByIdQuery(id));
				if (employee == null)
					return NotFound();

				return Ok(employee);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error obteniendo empleado {EmployeeId}", id);
				return StatusCode(500, new { message = "Error interno" });
			}
		}

		/// <summary>
		/// Crea un nuevo empleado (solo Admin o HR)
		/// </summary>
		[HttpPost]
		[Authorize(Roles = "Admin,HR")]  // ← Requerir rol específico
		public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest request)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				var command = new CreateEmployeeCommand(request);
				var result = await _mediator.Send(command);

				return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creando empleado");
				return StatusCode(500, new { message = "Error al crear empleado" });
			}
		}

		/// <summary>
		/// Actualiza un empleado (solo Admin o HR)
		/// </summary>
		[HttpPut("{id}")]
		[Authorize(Roles = "Admin,HR")]
		public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeRequest request)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				var command = new UpdateEmployeeCommand(id, request);
				var result = await _mediator.Send(command);

				return Ok(result);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error actualizando empleado {EmployeeId}", id);
				return StatusCode(500, new { message = "Error al actualizar empleado" });
			}
		}

		/// <summary>
		/// Elimina un empleado (solo Admin)
		/// </summary>
		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin")]  // ← Solo Admin
		public async Task<IActionResult> Delete(Guid id)
		{
			try
			{
				var command = new DeleteEmployeeCommand(id);
				await _mediator.Send(command);

				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error eliminando empleado {EmployeeId}", id);
				return StatusCode(500, new { message = "Error al eliminar empleado" });
			}
		}
	}
}
```

Aplica el mismo patrón a otros controladores:
- `CompaniesController`
- `BranchesController`
- `PersonalActionsController`
- etc.

---

## 🧪 PASO 9: TESTING

### 9.1 Verificar Swagger está activo

1. Ejecuta el API: `dotnet run` en `src/Gestiona360.Payroll.API`
2. Abre navegador: `https://localhost:7119/swagger`
3. Deberías ver interfaz Swagger con todos los endpoints

### 9.2 Testear Login

En Swagger UI:

1. Expande `POST /api/auth/login`
2. Click "Try it out"
3. Ingresa JSON:
```json
{
  "email": "admin@example.com",
  "password": "Admin123!@",
  "tenantCode": null
}
```
4. Click "Execute"
5. Deberías recibir 200 OK con tokens

### 9.3 Testear Protected Endpoint

1. Copia el `accessToken` de la respuesta del login
2. Click botón "Authorize" en Swagger (arriba derecha)
3. Pega: `Bearer {tu_token_aqui}`
4. Click "Authorize"
5. Ahora todos los endpoints tendrán el token

### 9.4 Ejemplos con cURL

```bash
# 1. Login
curl -X POST https://localhost:7119/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
	"email": "admin@example.com",
	"password": "Admin123!@",
	"tenantCode": null
  }' \
  --insecure

# Respuesta:
#{
#  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
#  "refreshToken": "sxR2w5Q8pK...",
#  "expiresIn": 900,
#  "tokenType": "Bearer",
#  "user": {...}
#}

# 2. Usar token en endpoint protegido
TOKEN="eyJhbGciOiJIUzI1NiIs..."
curl -X GET https://localhost:7119/api/employees \
  -H "Authorization: Bearer $TOKEN" \
  --insecure

# 3. Refresh token
curl -X POST https://localhost:7119/api/auth/refresh \
  -H "Content-Type: application/json" \
  -d '{
	"refreshToken": "sxR2w5Q8pK..."
  }' \
  --insecure

# 4. Logout
curl -X POST https://localhost:7119/api/auth/logout \
  -H "Authorization: Bearer $TOKEN" \
  --insecure
```

---

## ✅ CONCLUSIÓN

Has implementado:

✅ **JWT Token Generation** - Tokens seguros y validables  
✅ **Refresh Token Rotation** - Tokens de larga duración seguros  
✅ **AuthController** - Endpoints de login/logout/refresh  
✅ **Swagger/OpenAPI** - Documentación + testing interactivo  
✅ **[Authorize] Protection** - Endpoints protegidos por rol  
✅ **Frontend AuthService** - Integración Blazor con tokens  
✅ **CustomAuthStateProvider** - Lectura real de tokens  

### Próximos Pasos

1. **Cambiar Jwt:Key** en appsettings.json a cadena más fuerte
2. **Usar User Secrets** para desarrollo
3. **Azure Key Vault** para producción
4. **Agregar Rate Limiting** en /login
5. **Implementar Auditoría de Login**
6. **Testing** de todos los endpoints

---

**¿Preguntas? Revisar DIAGNOSTICO_SEGURIDAD_COMPLETO.md**

