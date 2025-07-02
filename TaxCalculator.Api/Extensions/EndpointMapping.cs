using Asp.Versioning;
using Asp.Versioning.Builder;
using TaxCalculator.Api.Endpoints;

namespace TaxCalculator.Api.Extensions;

internal static class EndpointMapping
{
    internal static void MapEndpoints(WebApplication app)
    {
        ApiVersionSet versionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .ReportApiVersions()
            .Build();

        app.MapEmployeeEndpoints(versionSet);
        app.MapTaxEndpoints(versionSet); 
    }
}
