using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TaxCalculator.Api.Data;
using TaxCalculator.Api.Repositories;
using TaxCalculator.Api.Services;
using TaxCalculator.Api.Models;

namespace TaxCalculator.Api.Extensions;

public static class MigrateAndSeedExtensions
{ 
    public static async Task MigrateAndSeedDatabaseAsync(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await db.Database.MigrateAsync();
        var seeder = scope.ServiceProvider.GetRequiredService<DataSeederService>();
        await seeder.SeedTaxBandsAsync();
        var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        if (env.IsDevelopment())
        {
            await seeder.SeedEmployeesAsync();
        }
    }
}
