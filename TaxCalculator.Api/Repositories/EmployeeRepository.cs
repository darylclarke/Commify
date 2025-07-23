using Microsoft.EntityFrameworkCore;
using TaxCalculator.Api.Data;
using TaxCalculator.Api.DTOs;
using TaxCalculator.Api.Models;

namespace TaxCalculator.Api.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EmployeeRepository> _logger;

    public EmployeeRepository(ApplicationDbContext context, ILogger<EmployeeRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<EmployeeDto>> GetEmployeesAsync(EmployeeQueryParameters parameters)
    {
        _logger.LogInformation("Getting employees from DB with parameters: {@Parameters}", parameters);
        var query = _context.Employees
            .AsQueryable()
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var searchTerm = parameters.SearchTerm.ToLower();
            query = query.Where(u => EF.Functions.Like(u.FullName.ToLower(), $"%{searchTerm}%"));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Salary = e.Salary,
            })
            .ToListAsync();

        _logger.LogInformation("Returning {Count} employees from DB", items.Count);
        return new PagedResult<EmployeeDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    public async Task<EmployeeDto?> GetEmployeeByIdAsync(Guid id)
    {
        _logger.LogInformation("Getting employee by id from DB: {Id}", id);
        var employee = await _context.Employees
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e => new EmployeeDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Salary = e.Salary,
            })
            .FirstOrDefaultAsync();

        if (employee == null)
            _logger.LogWarning("Employee not found in DB: {Id}", id);
        return employee;
    }

    public async Task<Employee?> UpdateEmployeeSalaryAsync(Guid id, int salary)
    {
        _logger.LogInformation("Updating salary for employee in DB {Id} to {Salary}", id, salary);
        var existingEmployee = await _context.Employees.FindAsync(id);
        if (existingEmployee == null)
        {
            _logger.LogWarning("Employee not found for update in DB: {Id}", id);
            return null;
        }
        existingEmployee.Salary = salary;
        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated salary for employee in DB {Id}", id);
        return existingEmployee;
    }
} 
