using Blazored.LocalStorage;
using Gestiona360.Payroll.Frontend;
using Gestiona360.Payroll.Frontend.Auth;
using Gestiona360.Payroll.Frontend.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// 1. MudBlazor
builder.Services.AddMudServices();

// 2. LocalStorage
builder.Services.AddBlazoredLocalStorage();

// 3. Autenticación
builder.Services.AddAuthorizationCore();

// ✅ REGISTRAR DE AMBAS FORMAS
builder.Services.AddScoped<JwtAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<JwtAuthStateProvider>());

// 4. Handler para agregar token a peticiones HTTP
builder.Services.AddScoped<AuthHeaderHandler>();

// 5. HttpClient con el Handler
builder.Services.AddHttpClient("Gestiona360API", client =>
{
    client.BaseAddress = new Uri("https://localhost:7119/"); // ⚠️ AJUSTA EL PUERTO
})
.AddHttpMessageHandler<AuthHeaderHandler>();

// 6. HttpClient predeterminado
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Gestiona360API"));

// 7. Servicios de la aplicación
builder.Services.AddScoped<AuthService>();
// builder.Services.AddScoped<EmployeeService>();

await builder.Build().RunAsync();