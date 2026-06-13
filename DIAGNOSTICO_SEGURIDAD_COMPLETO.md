# 🔐 DIAGNÓSTICO: Implementación de Seguridad, Login y Swagger
## Gestiona360.Payroll | .NET 10 | Blazor WASM + API

**Fecha del Diagnóstico:** 2025  
**Versión del Documento:** 1.0  
**Estado de la Aplicación:** ⚠️ Pre-Producción (sin seguridad implementada)

---

## 📋 TABLA DE CONTENIDOS

1. [Resumen Ejecutivo](#resumen-ejecutivo)
2. [Estado Actual del Sistema](#estado-actual-del-sistema)
3. [Análisis de Brechas de Seguridad](#análisis-de-brechas-de-seguridad)
4. [Recomendaciones de Seguridad](#recomendaciones-de-seguridad)
5. [Plan de Implementación](#plan-de-implementación)
6. [Mejores Prácticas](#mejores-prácticas)
7. [Checklist de Implementación](#checklist-de-implementación)
8. [Matriz de Riesgos](#matriz-de-riesgos)

---

## 🎯 RESUMEN EJECUTIVO

### Hallazgos Críticos

| Categoría | Estado | Severidad | Descripción |
|---|---|---|---|
| **Autenticación** | ❌ NO IMPLEMENTADA | CRÍTICA | Sin JWT, sin tokens, sin login real |
| **Autorización** | ⚠️ PARCIAL | ALTA | Identity sin claims/roles validados |
| **Documentación API** | ❌ SIN SWAGGER | MEDIA | Endpoints sin documentación interactiva |
| **Encriptación** | ⚠️ PARCIAL | ALTA | BD expuesta, contraseñas en plain |
| **Validación** | ⚠️ INCOMPLETA | MEDIA | Falta validación en endpoints |
| **Rate Limiting** | ❌ AUSENTE | MEDIA | Sin protección contra ataques |
| **Auditoría** | ❌ AUSENTE | MEDIA | Sin logs de autenticación |
| **HTTPS** | ⚠️ PARCIAL | MEDIA | CORS permite HTTP inseguro |

### Puntuación de Seguridad
```
┌─────────────────────────────┐
│  SEGURIDAD ACTUAL: 32/100   │
│  ████░░░░░░░░░░░░░░░░░░░░  │
└─────────────────────────────┘

Después de implementación: ~85/100
```

### Recomendación Principal
**CRÍTICO:** Implementar JWT + Login + Swagger **ANTES de producción**. El sistema actual es vulnerable a acceso no autorizado.

---

## 🏗️ ESTADO ACTUAL DEL SISTEMA

### 1. Arquitectura de Autenticación Actual

```
┌─────────────────────────────────────────────────────────┐
│                    FRONTEND (Blazor WASM)               │
│  - Login.razor (formulario sin backend)                │
│  - CustomAuthStateProvider (SIMULADO)                  │
│  - Sin AuthService                                      │
│  - Sin almacenamiento de tokens                        │
└──────────────────────┬──────────────────────────────────┘
					   │ Sin Bearer Token
┌──────────────────────▼──────────────────────────────────┐
│                   API (.NET 10)                         │
│  ✅ Identity Framework configurado                     │
│  ✅ Roles en BD                                         │
│  ✅ CORS habilitado                                     │
│  ❌ Sin JWT                                             │
│  ❌ Sin endpoints /login, /logout                       │
│  ❌ Sin Swagger                                         │
│  ❌ Sin [Authorize] en controladores                    │
└──────────────────────┬──────────────────────────────────┘
					   │ Sin validación
┌──────────────────────▼──────────────────────────────────┐
│              SQL Server (ApplicationDbContext)          │
│  - ApplicationUser (sin cambios)                        │
│  - Roles (sin usar)                                     │
│  - Credenciales en appsettings.json                    │
└─────────────────────────────────────────────────────────┘
```

### 2. Configuración Actual en Program.cs (API)

```csharp
// ✅ LO QUE EXISTE
builder.Services.AddCors(...);
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddOpenApi(); // ← Generic OpenAPI (NO ES SWAGGER)

app.UseAuthentication();  // ← Sin configuración JWT
app.UseAuthorization();
app.MapControllers();

// ❌ LO QUE FALTA
// AddAuthentication(JwtBearer)
// AddSwaggerGen()
// MapSwaggerUI()
// [Authorize] en endpoints
```

### 3. Estructura Identity

```csharp
// En DependencyInjection.cs
services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddDefaultTokenProviders();

// ✅ BIEN: Identity + Roles configurados
// ❌ FALTA: JWT, TokenGenerationService, RefreshToken
```

### 4. Estado del Login Frontend

```razor
<!-- Login.razor: Línea 83-146 -->
<MudTextField @bind-Value="email" Label="Correo Electrónico" />
<MudTextField @bind-Value="password" Label="Contraseña" InputType="InputType.Password" />

private async Task DoLogin()
{
	var result = await auth.Login(tenantCode, email, password);
	// ❌ auth.Login() no existe - referencia indefinida
	// ❌ La contraseña se envía en plain HTTP
}
```

### 5. CustomAuthStateProvider (Simulado)

```csharp
// Línea 1-32
public override async Task<AuthenticationState> GetAuthenticationStateAsync()
{
	// ⚠️ COMMENT DICE LO TODO:
	// "ESTA ES UNA IMPLEMENTACIÓN DE PRUEBA"
	// Simula usuario "UsuarioPrueba" con rol "Admin"

	var identity = new ClaimsIdentity(new[]
	{
		new Claim(ClaimTypes.Name, "UsuarioPrueba"),
		new Claim(ClaimTypes.Role, "Admin")
	}, "apiauth");

	// ❌ NO lee ningún token real
	// ❌ Siempre devuelve el mismo usuario
}
```

---

## ⚠️ ANÁLISIS DE BRECHAS DE SEGURIDAD

### BRECHA 1: Sin Autenticación de Usuarios

**Problema:**
```
┌──────────────────────────────────────────────┐
│ Cualquier usuario puede acceder a:           │
│ - GET /api/employees                         │
│ - POST /api/employees (crear)                │
│ - DELETE /api/employees/{id}                 │
│ Sin proporcionar ninguna credencial          │
└──────────────────────────────────────────────┘
```

**Impacto:** 
- ❌ Acceso no autorizado a datos sensibles
- ❌ Modificación de datos por usuarios no autorizados
- ❌ Violación de GDPR/privacidad

**Estado Actual:**
```
❌ Sin JWT tokens
❌ Sin validación de credenciales
❌ Sin endpoint /login
❌ Sin manejo de roles
```

---

### BRECHA 2: Credenciales Expuestas

**Problema:**
```json
// appsettings.json
{
  "ConnectionStrings": {
	"DefaultConnection": "Data Source=VPorras\\MSSQLSERVER2022;
						 User ID=sa;Password=Armagedon$70;"
  }
}
```

**Impacto:**
- ❌ Contraseña de BD visible en repositorio
- ❌ Si el repo es público: BD comprometida
- ❌ Violación de seguridad inmediata

**Recomendación:**
```json
// appsettings.json (producción)
{
  "ConnectionStrings": {
	"DefaultConnection": "User Secrets o Azure Key Vault"
  }
}
```

---

### BRECHA 3: Sin Documentación de API

**Problema:**
```
❌ No hay Swagger/OpenAPI
❌ Desarrolladores no saben qué endpoints existen
❌ Sin esquema de request/response
❌ Sin información de autenticación requerida
❌ Testing manual tedioso
```

**Endpoint Actual Sin Documentar:**
```
GET    /api/companies/main
PUT    /api/companies/main
GET    /api/employees
POST   /api/employees
PUT    /api/employees/{id}
DELETE /api/employees/{id}
... 23 endpoints más SIN DOCUMENTACIÓN
```

**Recomendación:** Swagger con Swashbuckle.AspNetCore

---

### BRECHA 4: Sin Rate Limiting

**Problema:**
```
❌ Alguien puede hacer 10,000 requests/segundo
❌ Ataques de fuerza bruta en login no controlados
❌ DoS (Denial of Service) posible
```

**Ejemplo de Ataque:**
```bash
while true; do
  curl -X POST https://api.payroll.com/api/auth/login \
	-d '{"email":"user@example.com","password":"intento123"}'
done
# Sin protección: 100,000+ intentos en minutos
```

---

### BRECHA 5: Sin Auditoría de Seguridad

**Problema:**
```
❌ No se registran intentos de login fallidos
❌ No se sabe quién accedió a qué datos
❌ No hay trazabilidad de cambios críticos
❌ Imposible investigar incidentes
```

**Impacto:** Imposible cumplir con auditoría, compliance, GDPR

---

## 🎯 RECOMENDACIONES DE SEGURIDAD

### RECOMENDACIÓN 1: Implementar JWT (JSON Web Tokens)

#### ¿Por qué JWT?
```
✅ Stateless: No requiere BD para validar cada request
✅ Escalable: Funciona bien con microservicios
✅ Seguro: Firmado digitalmente (HS256/RS256)
✅ Móvil-friendly: Funciona con cualquier cliente HTTP
✅ IETF RFC 7519: Standard oficial
```

#### Estructura de JWT
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9
.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ
.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c

ESTRUCTURA:
Header.Payload.Signature

HEADER (decodificado):
{
  "alg": "HS256",
  "typ": "JWT"
}

PAYLOAD (decodificado):
{
  "sub": "1234567890",         // Subject (user ID)
  "name": "John Doe",
  "iat": 1516239022,           // Issued At
  "exp": 1516242622,           // Expiration
  "roles": ["Admin", "HR"],
  "email": "john@example.com"
}

SIGNATURE:
HMACSHA256(base64UrlEncode(header) + "." + base64UrlEncode(payload), secret)
```

#### Flujo de Login con JWT
```
1. Usuario POST /api/auth/login (email + password)
   ↓
2. API valida contra Identity/BD
   ↓
3. Si válido: genera JWT (15 min) + RefreshToken (7 días)
   ↓
4. API retorna { "token": "eyJ...", "refreshToken": "...", "expiresIn": 900 }
   ↓
5. Frontend almacena tokens en localStorage
   ↓
6. Frontend adjunta token a cada request: Authorization: Bearer eyJ...
   ↓
7. API valida JWT (firma, expiracion, claims)
   ↓
8. Si válido: procesa request
   Si inválido: retorna 401 Unauthorized
```

---

### RECOMENDACIÓN 2: Implementar Refresh Tokens

**Problema sin Refresh Tokens:**
```
- Si JWT dura 7 días: sesiones largas inseguras
- Si JWT dura 15 min: usuario debe re-login cada 15 min (mala UX)
```

**Solución con Refresh Tokens:**
```
├─ Access Token (JWT corto: 15-60 min)
│  └─ Enviado en cada request
│     └─ Rápido de validar (JWT)
│
└─ Refresh Token (DB, largo: 7 días)
   └─ Usado SOLO cuando JWT expira
   └─ Endpoint /api/auth/refresh lo intercambia por JWT nuevo
   └─ Más seguro (menos exposición)
```

**Implementación:**
```csharp
// TokenGenerationService
public async Task<(string accessToken, string refreshToken)> GenerateTokensAsync(ApplicationUser user)
{
	// JWT corto (15 minutos)
	var accessToken = GenerateJwtToken(user, expirationMinutes: 15);

	// RefreshToken (7 días, guardado en BD)
	var refreshToken = GenerateRandomToken();
	var tokenEntity = new UserRefreshToken
	{
		UserId = user.Id,
		Token = refreshToken,
		ExpiresAt = DateTime.UtcNow.AddDays(7)
	};
	await _context.UserRefreshTokens.AddAsync(tokenEntity);
	await _context.SaveChangesAsync();

	return (accessToken, refreshToken);
}
```

---

### RECOMENDACIÓN 3: Proteger Endpoints con [Authorize]

**Patrón de Autorización por Rol:**
```csharp
[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
	// ✅ Cualquier usuario autenticado
	[Authorize]
	[HttpGet("{id}")]
	public async Task<IActionResult> GetEmployee(Guid id) { ... }

	// ✅ Solo Administradores
	[Authorize(Roles = "Admin")]
	[HttpPost]
	public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequest req) { ... }

	// ✅ Admin O Recursos Humanos
	[Authorize(Roles = "Admin,HR")]
	[HttpPut("{id}")]
	public async Task<IActionResult> UpdateEmployee(Guid id, ...) { ... }

	// ✅ Sin autenticación requerida
	[AllowAnonymous]
	[HttpPost("public-info")]
	public async Task<IActionResult> GetPublicInfo() { ... }
}
```

---

### RECOMENDACIÓN 4: Implementar Swagger/OpenAPI

**Beneficios:**
```
✅ Documentación automática de endpoints
✅ Interfaz interactiva para probar APIs
✅ Esquemas de request/response
✅ Información de autenticación requerida
✅ Cliente auto-generado para frontend
```

**Swagger UI:**
```
URL: https://api.payroll.com/swagger/index.html

Muestra:
- Todos los endpoints agrupados por controller
- Métodos HTTP (GET, POST, PUT, DELETE)
- Parámetros requeridos/opcionales
- Tipos de datos esperados
- Ejemplos de response
- Botón "Try it out" para probar
- Soporte para autorización Bearer
```

---

### RECOMENDACIÓN 5: Implementar Auditoría de Login

**Tabla de Auditoría:**
```sql
CREATE TABLE LoginAuditLogs (
	Id BIGINT PRIMARY KEY IDENTITY(1,1),
	UserId UNIQUEIDENTIFIER NOT NULL,
	Email NVARCHAR(256) NOT NULL,
	LoginTime DATETIME2 NOT NULL,
	Success BIT NOT NULL,
	FailureReason NVARCHAR(MAX),
	IpAddress NVARCHAR(45),
	UserAgent NVARCHAR(MAX),
	TenantCode NVARCHAR(50)
);
```

**Uso:**
```csharp
// En AuthController.Login()
try
{
	var user = await _userManager.FindByEmailAsync(email);
	if (user != null && await _userManager.CheckPasswordAsync(user, password))
	{
		await _auditService.LogLoginAsync(user.Id, email, success: true, ipAddress);
		// Generar JWT...
	}
	else
	{
		await _auditService.LogLoginAsync(null, email, success: false, 
			"Invalid credentials", ipAddress);
		return Unauthorized();
	}
}
```

---

### RECOMENDACIÓN 6: Validación en Frontend

**Almacenamiento Seguro de Tokens:**
```csharp
// ✅ RECOMENDADO: sessionStorage (limpiado al cerrar navegador)
await JS.InvokeVoidAsync("sessionStorage.setItem", "token", jwtToken);

// ⚠️ ACEPTABLE: localStorage (persistent, pero más riesgo)
await JS.InvokeVoidAsync("localStorage.setItem", "token", jwtToken);

// ❌ NUNCA: Variables de memoria sin lógica
```

**HttpClient Interceptor:**
```csharp
// En AuthService
private async Task<HttpResponseMessage> SendWithTokenAsync(
	HttpRequestMessage request)
{
	var token = await GetTokenAsync();
	if (!string.IsNullOrEmpty(token))
	{
		request.Headers.Authorization = 
			new AuthenticationHeaderValue("Bearer", token);
	}

	var response = await _httpClient.SendAsync(request);

	// Si 401: intentar refresh
	if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
	{
		await RefreshTokenAsync();
		token = await GetTokenAsync();
		request.Headers.Authorization = 
			new AuthenticationHeaderValue("Bearer", token);
		response = await _httpClient.SendAsync(request);
	}

	return response;
}
```

---

## 📊 PLAN DE IMPLEMENTACIÓN

### FASE 1: Infraestructura JWT (3-4 horas)
**Ubicación:** `src/Gestiona360.Payroll.Infrastructure.Identity/`

**Archivos a Crear:**
```
✅ TokenGenerationService.cs
   - GenerateJwtToken(user, roles)
   - GenerateRefreshToken()
   - ValidateRefreshToken(token)

✅ UserRefreshTokenEntity.cs
   - Para almacenar refresh tokens en BD

✅ DependencyInjection update
   - Registrar TokenGenerationService
   - Configurar JWT options
```

**NuGet Packages:**
```
- System.IdentityModel.Tokens.Jwt (>= 8.0.0)
- Microsoft.IdentityModel.Tokens (>= 8.0.0)
```

---

### FASE 2: DTOs de Autenticación (1 hora)
**Ubicación:** `src/Gestiona360.Payroll.Application.Contracts/DTOs/Auth/`

**Archivos a Crear:**
```
✅ LoginRequest.cs
   {
	 "email": "user@example.com",
	 "password": "SecurePassword123!",
	 "tenantCode": "EMPRESA001"
   }

✅ LoginResponse.cs
   {
	 "accessToken": "eyJ...",
	 "refreshToken": "eyJ...",
	 "expiresIn": 900,
	 "user": { "id": "...", "email": "...", "roles": [...] }
   }

✅ RefreshTokenRequest.cs
   {
	 "refreshToken": "eyJ..."
   }

✅ UserDto.cs (para respuesta)
   {
	 "id": "...",
	 "email": "...",
	 "firstName": "...",
	 "lastName": "...",
	 "roles": ["Admin", "HR"]
   }
```

---

### FASE 3: CQRS Commands (2 horas)
**Ubicación:** `src/Gestiona360.Payroll.Application/Features/Auth/`

**Archivos a Crear:**
```
✅ LoginCommand.cs (IRequest<LoginResponse>)
   public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
   {
	   public async Task<LoginResponse> Handle(LoginCommand request, ...)
	   {
		   // Validar usuario
		   // Validar contraseña
		   // Generar tokens
		   // Retornar LoginResponse
	   }
   }

✅ RefreshTokenCommand.cs (IRequest<LoginResponse>)
   public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResponse>
   {
	   public async Task<LoginResponse> Handle(RefreshTokenCommand request, ...)
	   {
		   // Validar refresh token
		   // Generar nuevo access token
		   // Retornar LoginResponse
	   }
   }

✅ LogoutCommand.cs
   public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
   {
	   public async Task Handle(LogoutCommand request, ...)
	   {
		   // Invalidar refresh token en BD
		   // Log auditoría
	   }
   }
```

---

### FASE 4: AuthController (2 horas)
**Ubicación:** `src/Gestiona360.Payroll.API/Controllers/AuthController.cs`

```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
	private readonly IMediator _mediator;
	private readonly ILogger<AuthController> _logger;

	[AllowAnonymous]
	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] LoginRequest request)
	{
		// POST /api/auth/login
		// 200 OK + LoginResponse
		// 401 Unauthorized
		// 400 Bad Request
	}

	[Authorize]
	[HttpPost("refresh")]
	public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
	{
		// POST /api/auth/refresh
		// 200 OK + LoginResponse con nuevo token
		// 401 Unauthorized si refresh token inválido
	}

	[Authorize]
	[HttpPost("logout")]
	public async Task<IActionResult> Logout()
	{
		// POST /api/auth/logout
		// 200 OK (siempre, incluso sin token)
	}

	[Authorize]
	[HttpGet("me")]
	public async Task<IActionResult> GetCurrentUser()
	{
		// GET /api/auth/me
		// 200 OK + UserDto del usuario actual
		// 401 si no autenticado
	}
}
```

---

### FASE 5: Configurar JWT en Program.cs (1 hora)

```csharp
// AGREGAR EN Program.cs (API)

// 1. Leer configuración JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = Encoding.ASCII.GetBytes(
	jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not configured"));

// 2. Registrar autenticación JWT
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

// 3. Agregar Swagger con soporte JWT
builder.Services.AddSwaggerGen(c =>
{
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
				}
			},
			Array.Empty<string>()
		}
	});
});

// 4. En middleware
app.UseAuthentication();  // ANTES de UseAuthorization
app.UseAuthorization();
```

---

### FASE 6: Configurar Swagger/OpenAPI (30 min)

```csharp
// AGREGAR EN Program.cs

builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "Gestiona360 Payroll API",
		Version = "1.0",
		Description = "API de nómina con autenticación JWT",
		Contact = new OpenApiContact
		{
			Name = "Tu Empresa",
			Email = "support@empresa.com"
		}
	});

	// Documentar endpoints con [ProducesResponseType]
	c.OperationFilter<AuthorizeCheckOperationFilter>();
});

// En middleware (si IsDevelopment)
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gestiona360 API v1");
		c.RoutePrefix = "swagger"; // URL: /swagger
	});
}
```

---

### FASE 7: AuthService en Frontend (2-3 horas)
**Ubicación:** `src/Gestiona360.Payroll.Frontend/Services/AuthService.cs`

```csharp
public class AuthService
{
	private readonly HttpClient _httpClient;
	private readonly IJSRuntime _js;
	private const string TokenKey = "auth_token";
	private const string RefreshTokenKey = "refresh_token";

	public async Task<(bool Success, string? Error)> LoginAsync(
		string email, 
		string password, 
		string tenantCode)
	{
		try
		{
			var request = new { email, password, tenantCode };
			var response = await _httpClient.PostAsJsonAsync(
				"api/auth/login", request);

			if (response.IsSuccessStatusCode)
			{
				var result = await response.Content
					.ReadAsAsync<LoginResponse>();

				// Guardar tokens
				await _js.InvokeVoidAsync("sessionStorage.setItem", 
					TokenKey, result.AccessToken);
				await _js.InvokeVoidAsync("sessionStorage.setItem", 
					RefreshTokenKey, result.RefreshToken);

				return (true, null);
			}
			else
			{
				var error = await response.Content.ReadAsStringAsync();
				return (false, error);
			}
		}
		catch (Exception ex)
		{
			return (false, ex.Message);
		}
	}

	public async Task LogoutAsync()
	{
		try
		{
			await _httpClient.PostAsync("api/auth/logout", null);
		}
		finally
		{
			await _js.InvokeVoidAsync("sessionStorage.removeItem", TokenKey);
			await _js.InvokeVoidAsync("sessionStorage.removeItem", RefreshTokenKey);
		}
	}

	public async Task<string?> GetTokenAsync()
	{
		return await _js.InvokeAsync<string>(
			"sessionStorage.getItem", TokenKey);
	}

	public async Task<bool> RefreshTokenAsync()
	{
		var refreshToken = await _js.InvokeAsync<string>(
			"sessionStorage.getItem", RefreshTokenKey);

		if (string.IsNullOrEmpty(refreshToken))
			return false;

		var response = await _httpClient.PostAsJsonAsync(
			"api/auth/refresh", 
			new { refreshToken });

		if (response.IsSuccessStatusCode)
		{
			var result = await response.Content
				.ReadAsAsync<LoginResponse>();

			await _js.InvokeVoidAsync("sessionStorage.setItem", 
				TokenKey, result.AccessToken);

			return true;
		}

		return false;
	}

	public async Task<bool> IsAuthenticatedAsync()
	{
		var token = await GetTokenAsync();
		return !string.IsNullOrEmpty(token);
	}
}
```

---

### FASE 8: Mejorar CustomAuthStateProvider (1 hora)

```csharp
public class CustomAuthStateProvider : AuthenticationStateProvider
{
	private readonly IJSRuntime _js;
	private readonly AuthService _authService;

	public override async Task<AuthenticationState> GetAuthenticationStateAsync()
	{
		try
		{
			var token = await _authService.GetTokenAsync();

			if (string.IsNullOrEmpty(token))
			{
				return new AuthenticationState(new ClaimsPrincipal());
			}

			var claims = ParseClaimsFromJwt(token);
			var identity = new ClaimsIdentity(claims, "jwt");
			var user = new ClaimsPrincipal(identity);

			return new AuthenticationState(user);
		}
		catch
		{
			return new AuthenticationState(new ClaimsPrincipal());
		}
	}

	private static List<Claim> ParseClaimsFromJwt(string jwt)
	{
		var payload = jwt.Split('.')[1];
		var jsonBytes = ParseBase64WithoutPadding(payload);
		var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

		var claims = new List<Claim>();
		keyValuePairs?.ForEach(kvp => claims.Add(
			new Claim(kvp.Key, kvp.Value.ToString() ?? "")));

		return claims;
	}

	private static byte[] ParseBase64WithoutPadding(string base64)
	{
		switch (base64.Length % 4)
		{
			case 2: base64 += "=="; break;
			case 3: base64 += "="; break;
		}
		return Convert.FromBase64String(base64);
	}
}
```

---

### FASE 9: Proteger Controladores (30 min)

```csharp
// Ejemplo: EmployeesController

[ApiController]
[Route("api/[controller]")]
[Authorize] // ← AGREGAR AQUÍ
public class EmployeesController : ControllerBase
{
	[HttpGet]
	public async Task<IActionResult> GetAll() { ... }

	[HttpGet("{id}")]
	public async Task<IActionResult> GetById(Guid id) { ... }

	[HttpPost]
	[Authorize(Roles = "Admin,HR")] // ← Solo ciertos roles
	public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest req) { ... }

	[HttpPut("{id}")]
	[Authorize(Roles = "Admin,HR")]
	public async Task<IActionResult> Update(Guid id, ...) { ... }

	[HttpDelete("{id}")]
	[Authorize(Roles = "Admin")] // ← Solo Admin
	public async Task<IActionResult> Delete(Guid id) { ... }
}
```

---

## ✅ MEJORES PRÁCTICAS

### 1. Gestión de Secretos
```
❌ NUNCA: Secretos en appsettings.json
✅ SIEMPRE: 
   - Desarrollo: dotnet user-secrets
   - Producción: Azure Key Vault / AWS Secrets Manager
```

### 2. Contraseñas Fuertes
```csharp
var passwordHasher = new PasswordHasher<ApplicationUser>();
var hashedPassword = passwordHasher.HashPassword(user, plainPassword);
// Usar Identity.PasswordHasher (bcrypt-like)
```

### 3. HTTPS Enforce
```csharp
if (!app.Environment.IsDevelopment())
{
	app.UseHttpsRedirection();
}
```

### 4. CORS Restrictivo
```csharp
// ❌ MAL
options.AllowAnyOrigin();

// ✅ BIEN
options.WithOrigins("https://payroll.example.com");
```

### 5. Validación de Input
```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] LoginRequest request)
	{
		// VALIDAR
		if (!ModelState.IsValid)
			return BadRequest(ModelState);

		if (string.IsNullOrWhiteSpace(request.Email) || 
			!request.Email.Contains("@"))
			return BadRequest("Email inválido");

		if (request.Password.Length < 8)
			return BadRequest("Contraseña muy corta");

		// ... resto
	}
}
```

### 6. Logging Seguro
```csharp
_logger.LogInformation($"User {email} logged in successfully");
_logger.LogWarning($"Failed login attempt for {email}"); // NO log del password
_logger.LogError("Database connection failed"); // NO credenciales en logs
```

### 7. Rate Limiting
```csharp
// NuGet: AspNetCoreRateLimit
builder.Services.AddMemoryCache();
builder.Services.AddInMemoryRateLimiting();

// En appsettings.json
{
  "IpRateLimiting": {
	"EnableEndpointRateLimiting": true,
	"StackBlockedRequests": false,
	"RealIpHeader": "X-Real-IP",
	"GeneralRules": [
	  {
		"Endpoint": "POST:/api/auth/login",
		"Period": "1m",
		"Limit": 5  // 5 intentos por minuto
	  }
	]
  }
}
```

---

## 📋 CHECKLIST DE IMPLEMENTACIÓN

### FASE 1: Infraestructura
```
□ Crear UserRefreshToken entity
□ Crear TokenGenerationService
□ Registrar en DependencyInjection
□ Agregar NuGet: System.IdentityModel.Tokens.Jwt
□ Configurar JWT en appsettings.json
```

### FASE 2: DTOs
```
□ LoginRequest.cs
□ LoginResponse.cs
□ RefreshTokenRequest.cs
□ UserDto.cs
□ Validaciones en DTOs
```

### FASE 3: CQRS
```
□ LoginCommand + Handler
□ RefreshTokenCommand + Handler
□ LogoutCommand + Handler
□ Registrar en DependencyInjection
```

### FASE 4: AuthController
```
□ POST /api/auth/login
□ POST /api/auth/refresh
□ POST /api/auth/logout
□ GET /api/auth/me
□ Validaciones
□ Error handling
```

### FASE 5: Configurar JWT
```
□ AddAuthentication(JwtBearer) en Program.cs
□ TokenValidationParameters configurados
□ AddSwaggerGen con Bearer scheme
□ UseAuthentication() en middleware
□ UseAuthorization() en middleware
```

### FASE 6: Swagger
```
□ AddSwaggerGen configurado
□ UseSwagger() en middleware
□ UseSwaggerUI() en middleware
□ Documentar endpoints con [ProducesResponseType]
□ Verificar en /swagger
```

### FASE 7: Frontend
```
□ AuthService.cs creado
□ Login method
□ Logout method
□ GetToken method
□ RefreshToken method
□ Token storage en sessionStorage
```

### FASE 8: CustomAuthStateProvider
```
□ Leer token de storage
□ Parsear claims del JWT
□ Crear ClaimsIdentity
□ Devolver ClaimsPrincipal
```

### FASE 9: Proteger Controladores
```
□ [Authorize] en todos los endpoints sensibles
□ [Authorize(Roles = "...")] para endpoints específicos
□ [AllowAnonymous] solo donde sea necesario
□ Testear 401 sin autenticación
```

### FASE 10: Documentación
```
□ DIAGNOSTICO_SEGURIDAD.md (este archivo ✓)
□ GUIA_IMPLEMENTACION_JWT.md (código paso a paso)
□ EJEMPLOS_CURL.md (comandos de prueba)
□ README.md actualizado
```

### FASE 11: Testing
```
□ Probar login exitoso
□ Probar login fallido
□ Probar token expiracion
□ Probar refresh token
□ Probar logout
□ Probar endpoints sin autenticación (401)
□ Probar endpoints sin rol requerido (403)
□ Probar Swagger con Bearer token
```

### FASE 12: Seguridad
```
□ Cambiar contraseña de BD en appsettings.json
□ Usar User Secrets en desarrollo
□ HTTPS enforced en producción
□ CORS restrictivo
□ Rate limiting en /login
□ Logs de auditoría
```

---

## 🚨 MATRIZ DE RIESGOS

### Riesgo 1: Breach de Datos (Acceso No Autorizado)
```
SEVERIDAD: 🔴 CRÍTICA
PROBABILIDAD: 🔴 MUY ALTA (sin JWT implementado)

IMPACTO:
- Exposición de datos sensibles de empleados
- Violación de GDPR / privacidad
- Demandas legales
- Pérdida de confianza

MITIGACIÓN:
✅ Implementar JWT inmediatamente
✅ [Authorize] en todos los endpoints
✅ Auditoría de acceso
✅ Auditoría de cambios
```

### Riesgo 2: Ataque de Fuerza Bruta
```
SEVERIDAD: 🟠 ALTA
PROBABILIDAD: 🟠 ALTA (sin rate limiting)

IMPACTO:
- Compromiso de cuenta de usuario
- Acceso no autorizado

MITIGACIÓN:
✅ Rate limiting (5 intentos / minuto)
✅ Account lockout después de 3 fallos
✅ Monitoreo de intentos fallidos
```

### Riesgo 3: Contraseñas Débiles
```
SEVERIDAD: 🟠 ALTA
PROBABILIDAD: 🟡 MEDIA

IMPACTO:
- Compromiso fácil de cuentas
- Acceso no autorizado

MITIGACIÓN:
✅ Validación de contraseña fuerte (UpperCase, Number, Symbol)
✅ Mínimo 8 caracteres
✅ Usar PasswordHasher de Identity
```

### Riesgo 4: Token Robado
```
SEVERIDAD: 🟠 ALTA
PROBABILIDAD: 🟡 MEDIA

IMPACTO:
- XSS en frontend → token robado
- Acceso impersonado

MITIGACIÓN:
✅ Almacenar en sessionStorage (no localStorage)
✅ HTTPS siempre
✅ Short-lived JWT (15 min)
✅ RefreshToken rotado regularmente
```

### Riesgo 5: Inyección SQL
```
SEVERIDAD: 🔴 CRÍTICA
PROBABILIDAD: 🟢 BAJA (usando Entity Framework)

MITIGACIÓN:
✅ Entity Framework Core → queries parametrizadas
✅ Validación de input
✅ Nunca concatenar strings en queries
```

---

## 🔍 TABLA COMPARATIVA: ANTES vs DESPUÉS

| Aspecto | ANTES ❌ | DESPUÉS ✅ |
|---|---|---|
| **Autenticación** | Sin JWT, simulada | JWT + Refresh Token |
| **Login** | Sin endpoint real | POST /api/auth/login |
| **Tokens** | Ninguno | Access Token + Refresh Token |
| **Endpoints Protegidos** | Sin [Authorize] | Con [Authorize] + [Authorize(Roles)] |
| **Swagger** | Sin documentación | Swagger UI completo + JWT |
| **Auditoría** | Sin logs | LoginAuditLogs + cambios |
| **Rate Limiting** | Sin protección | 5 intentos/min en login |
| **Secretos** | En appsettings.json | En User Secrets / Key Vault |
| **HTTPS** | No obligatorio | Obligatorio en prod |
| **Scoring** | 32/100 🔴 | 85/100 🟢 |

---

## 📞 SIGUIENTE PASO

**Cuando esté listo para implementar, siga:**
1. Leer `GUIA_IMPLEMENTACION_JWT.md` (código detallado)
2. Crear archivos en orden especificado
3. Ejecutar tests en `EJEMPLOS_CURL.md`
4. Validar seguridad con checklist
5. Documentar cambios en README

---

## 📚 REFERENCIAS

### RFC Standards
- RFC 7519: JSON Web Token (JWT)
- RFC 7231: HTTP Semantics and Content
- RFC 6234: Password-Based Key Derivation Function 2 (PBKDF2)

### Microsoft Docs
- [Authorize with JWT in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwt-authn)
- [Identity in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity)
- [Swagger/OpenAPI in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger)

### Libraries
- System.IdentityModel.Tokens.Jwt
- Swashbuckle.AspNetCore
- AspNetCoreRateLimit
- Microsoft.AspNetCore.DataProtection

---

**Documento Generado:** 2025  
**Versión:** 1.0  
**Estado:** ⏳ Esperando aprobación para implementación

