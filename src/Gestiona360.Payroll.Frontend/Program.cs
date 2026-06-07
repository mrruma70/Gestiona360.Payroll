using Gestiona360.Payroll.Frontend;
using Gestiona360.Payroll.Frontend.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;


// program.cs frontend
var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");

builder.Services.AddMudServices();

// Servicios de Catálogos


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
//builder.Services.AddScoped<ICatalogService, CatalogApiService>();

// HttpClient CORRECTO
builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri("https://localhost:7119/")
    });

// Servicio
builder.Services.AddScoped<PayrollService>();

await builder.Build().RunAsync();
