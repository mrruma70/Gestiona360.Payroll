using Gestiona360.Payroll.Application;
using Gestiona360.Payroll.Application.Features.Employees.Exports;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Infrastructure.Identity;
using Gestiona360.Payroll.Infrastructure.Persistence;
using Gestiona360.Payroll.Infrastructure.Persistence.Seeding;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// API
var builder = WebApplication.CreateBuilder(args);

// CORS
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

// Registrar Persistencia (que ahora configura internamente el DbContext e Identity)
builder.Services.AddApplication();                
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddIdentityInfrastructure(builder.Configuration);


//builder.Services.AddScoped<ICatalogRepository, CatalogRepository>();
//builder.Services.AddScoped<ICatalogService, CatalogService>();

//builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MediatRMarker).Assembly));
//builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));



builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();


// Bloque de inicialización de Base de Datos en el ciclo de vida del Startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        // 1. Aplica migraciones pendientes de forma automática sin bloquear la base de datos
        await context.Database.MigrateAsync();

        // Sembrar roles y usuario administrador
        await ApplicationDbInitializer.InitializeIdentityAsync(context, userManager, roleManager, logger);

        // 2. Ejecuta el sembrado de datos por defecto si la base está vacía
        await DbInitializer.SeedAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error fatal al migrar o sembrar la base de datos.");
    }
}

if (app.Environment.IsDevelopment())
{
    //using (var scope = app.Services.CreateScope())
    //{
    //    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    //    dbContext.Database.Migrate(); // ? esto aplica migraciones pendientes automáticamente

    //    // 2. Ejecuta el sembrado de datos por defecto si la base está vacía
    //    await DbInitializer.SeedAsync(dbContext);
    //}
    app.MapOpenApi();
}

app.UseStaticFiles();

app.UseCors("AllowBlazorClient");
app.UseHttpsRedirection();

// El orden de los middlewares es crucial
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();