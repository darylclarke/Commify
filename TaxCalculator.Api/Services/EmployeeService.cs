using TaxCalculator.Api.DTOs;
using TaxCalculator.Api.Repositories;
using TaxCalculator.Api.Models;
using Microsoft.Extensions.Logging;

namespace TaxCalculator.Api.Services;

public class EmployeeService(IEmployeeRepository employeeRepository, ILogger<EmployeeService> logger) : IEmployeeService
{
    public async Task<PagedResult<EmployeeDto>> GetEmployeesAsync(EmployeeQueryParameters parameters)
    {
        logger.LogInformation("Getting employees with parameters: {@Parameters}", parameters);
        // Validate parameters
        if (parameters.PageNumber < 1)
            parameters.PageNumber = 1;
        
        if (parameters.PageSize < 1 || parameters.PageSize > 100)
            parameters.PageSize = 10;

        var result = await employeeRepository.GetEmployeesAsync(parameters);
        return result;
    }

    public async Task<EmployeeDto?> GetEmployeeByIdAsync(Guid id)
    {
        logger.LogInformation("Getting employee by id: {Id}", id);
        var employee = await employeeRepository.GetEmployeeByIdAsync(id);
        if (employee == null)
            logger.LogWarning("Employee not found: {Id}", id);
        return employee;
    }

    public async Task<EmployeeDto?> UpdateEmployeeSalaryAsync(Guid id, int salary)
    {
        logger.LogInformation("Updating salary for employee {Id} to {Salary}", id, salary);
        var updatedEmployee = await employeeRepository.UpdateEmployeeSalaryAsync(id, salary);
        
        if (updatedEmployee == null)
        {
            logger.LogWarning("Employee not found for update: {Id}", id);
            return null;
        }
        logger.LogInformation("Updated salary for employee {Id}", id);
        return new EmployeeDto
        {
            Id = updatedEmployee.Id,
            FirstName = updatedEmployee.FirstName,
            LastName = updatedEmployee.LastName,
            Salary = updatedEmployee.Salary,
        };
    }
}
