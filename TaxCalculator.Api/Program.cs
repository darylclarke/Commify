using Serilog;
using TaxCalculator.Api.Extensions;
using TaxCalculator.Api.Services;
using TaxCalculator.Api.Repositories;

var logsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
if (!Directory.Exists(logsDirectory))
{
    Directory.CreateDirectory(logsDirectory);
}

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(Path.Combine(logsDirectory, "taxcalculatorapi.txt"), rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

Log.Information("Starting up");
try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddApplicationServices(builder.Configuration);
    
    builder.Services.AddOpenApi();

    var app = builder.Build();

    app.UseMiddleware<TaxCalculator.Api.Middleware.ExceptionMiddleware>();

    await app.MigrateAndSeedDatabaseAsync();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }
    
    EndpointMapping.MapEndpoints(app);

    app.UseCors();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

