using Asp.Versioning.Builder;
using TaxCalculator.Api.DTOs;
using TaxCalculator.Api.Services;

namespace TaxCalculator.Api.Endpoints;

public static class TaxEndpoints
{
    public static void MapTaxEndpoints(this WebApplication app, ApiVersionSet versionset)
    {
        RouteGroupBuilder group = app
            .MapGroup("api/v{version:apiVersion}/tax")
            .WithApiVersionSet(versionset);
        
        group.MapGet("/calculate/{salary}", async (int salary, ITaxCalculationService taxService, CancellationToken cancellationToken) =>
            {
                if (salary < 0)
                {
                    return Results.BadRequest("Salary cannot be negative");
                }
                var result = await taxService.CalculateTaxAsync(salary, cancellationToken);
                return Results.Ok(result);
            })
            .WithName("CalculateTax")
            .WithOpenApi();

    }
}
