using Asp.Versioning.Builder;
using TaxCalculator.Api.DTOs;
using TaxCalculator.Api.Services;

namespace TaxCalculator.Api.Endpoints;

public static class EmployeeEndpoints
{
    public static void MapEmployeeEndpoints(this WebApplication app, ApiVersionSet versionset)
    {
        RouteGroupBuilder group = app
            .MapGroup("api/v{version:apiVersion}")
            .WithApiVersionSet(versionset);
        
        group.MapGet("/employees", async (
                [AsParameters] EmployeeQueryParameters parameters,
                IEmployeeService employeeService) =>
            {
                var result = await employeeService.GetEmployeesAsync(parameters);
                return Results.Ok(result);
            })
            .WithName("GetEmployees")
            .WithOpenApi();

        group.MapGet("/employee/{id}", async (Guid id, IEmployeeService employeeService) =>
            {
                var employee = await employeeService.GetEmployeeByIdAsync(id);
                if (employee == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(employee);
            })
            .WithName("GetEmployee")
            .WithOpenApi();

        group.MapPut("employee/{id}", async (Guid id, int salary, IEmployeeService employeeService) =>
            {
                var employee = await employeeService.UpdateEmployeeSalaryAsync(id, salary);
                if (employee == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(employee);
            })
            .WithName("UpdateEmployeeSalary")
            .WithOpenApi();

    }
}
