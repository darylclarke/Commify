using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TaxCalculator.Api.Data;
using TaxCalculator.Api.Repositories;
using TaxCalculator.Api.Services;
using TaxCalculator.Api.Models;

namespace TaxCalculator.Api.Extensions;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        var sqlConnectionString = configuration.GetConnectionString("DBConnection") ??
                                  throw new Exception("DBConnection is missing");

        services.AddDbContext<ApplicationDbContext>(opt => { opt.UseSqlite(sqlConnectionString); });
        services.AddMemoryCache();
        services.AddHealthChecks()
            .AddSqlite(sqlConnectionString)
            .AddCheck("self", () => HealthCheckResult.Healthy());

        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<ITaxCalculationService, TaxCalculationService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<DataSeederService>();

        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(allowedOrigins ?? [])
                      .WithMethods("PUT", "GET");
            });
        });

        services.AddApiVersioning();

        services.Configure<CacheSettings>(configuration.GetSection("CacheSettings"));

        return services;
    }

    public static async Task MigrateAndSeedDatabaseAsync(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
        var seeder = scope.ServiceProvider.GetRequiredService<DataSeederService>();
        await seeder.SeedTaxBandsAsync();
        var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        if (env.IsDevelopment())
        {
            await seeder.SeedEmployeesAsync();
        }
    }
}
