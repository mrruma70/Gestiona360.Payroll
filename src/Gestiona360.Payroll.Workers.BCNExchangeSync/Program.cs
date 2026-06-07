using Gestiona360.Payroll.Workers.BCNExchangeSync;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
