using TaxCalculator.Api.DTOs;
using TaxCalculator.Api.Models;

namespace TaxCalculator.Api.Repositories;

public interface IEmployeeRepository
{
    Task<PagedResult<EmployeeDto>> GetEmployeesAsync(EmployeeQueryParameters parameters);
    Task<EmployeeDto?> GetEmployeeByIdAsync(Guid id);
    Task<Employee?> UpdateEmployeeSalaryAsync(Guid id, int salary);
} 