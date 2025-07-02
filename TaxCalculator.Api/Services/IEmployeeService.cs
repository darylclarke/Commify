using TaxCalculator.Api.DTOs;
using TaxCalculator.Api.Models;

namespace TaxCalculator.Api.Services;

public interface IEmployeeService
{
    Task<PagedResult<EmployeeDto>> GetEmployeesAsync(EmployeeQueryParameters parameters);
    Task<EmployeeDto?> GetEmployeeByIdAsync(Guid id);
    Task<EmployeeDto?> UpdateEmployeeSalaryAsync(Guid id, int salary);
} 