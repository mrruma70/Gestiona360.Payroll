using System.Security.Claims;
using System.Text;
using Gestiona360.Payroll.Application;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Infrastructure.Identity;
using Gestiona360.Payroll.Infrastructure.Persistence;
using Gestiona360.Payroll.Infrastructure.Persistence.Seeding;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// CORS
// ============================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("http://localhost:5063", "https://localhost:7141")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .WithExposedHeaders("Content-Disposition");
    });
});

// ============================================
// SERVICIOS DE LA APLICACIÓN
// ============================================
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddHttpContextAccessor();


var jwtKey = builder.Configuration["Jwt:Key"];
var keyBytes = Convert.FromBase64String(jwtKey);
var signingKey = new SymmetricSecurityKey(keyBytes);
Console.WriteLine($"Clave decodificada: {BitConverter.ToString(keyBytes)}");
Console.WriteLine($"Longitud: {keyBytes.Length} bytes"); // Debe ser 32
// ============================================
// JWT AUTHENTICATION
// ============================================
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = signingKey,
        //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero,
        NameClaimType = ClaimTypes.Name,
        RoleClaimType = ClaimTypes.Role
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"❌ JWT Error: {context.Exception.Message}");
            if (context.Exception.InnerException != null)
                Console.WriteLine($"   Inner: {context.Exception.InnerException.Message}");
            Console.WriteLine($"   StackTrace: {context.Exception.StackTrace}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// ============================================
// OPENAPI NATIVO DE .NET 10 (Reemplaza Swagger)
// ============================================
builder.Services.AddOpenApi();

// ============================================
// CONTROLLERS
// ============================================
builder.Services.AddControllers();

// ============================================
// APLICACIÓN
// ============================================
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

// ============================================
// MIDDLEWARE PIPELINE
// https://localhost:7119/scalar/v1
// https://localhost:7119/openapi/v1.json
// ============================================
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // Endpoint: /openapi/v1.json
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Gestiona 360 - Payroll API");
        options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

// ✅ ORDEN CORRECTO DEL PIPELINE
app.UseHttpsRedirection();      // 1. PRIMERO: Redirección HTTPS
app.UseStaticFiles();           // 2. Archivos estáticos
app.UseCors("AllowBlazorClient"); // 3. CORS DESPUÉS de HTTPS

// Middleware de logging (opcional)
app.Use(async (context, next) =>
{
    var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
    Console.WriteLine($"🔐 Header recibido: {authHeader}");
    if (!string.IsNullOrEmpty(authHeader))
    {
        var token = authHeader.Substring("Bearer ".Length);
        Console.WriteLine($"Token: {token.Substring(0, Math.Min(50, token.Length))}...");
    }
    await next();
});

// 4. Autenticación y Autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();