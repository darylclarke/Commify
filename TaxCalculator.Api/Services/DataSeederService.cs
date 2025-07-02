using TaxCalculator.Api.Data;
using TaxCalculator.Api.Models;

namespace TaxCalculator.Api.Services;

public class DataSeederService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DataSeederService> _logger;

    public DataSeederService(ApplicationDbContext context, ILogger<DataSeederService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedTaxBandsAsync()
    {
        _logger.LogInformation("Seeding tax bands if none exist.");
        if (!_context.TaxBands.Any())
        {
            var taxBands = new List<TaxBand>
            {
                new TaxBand
                {
                    Id = Guid.NewGuid(),
                    Name = "Tax Band A",
                    Range = new int[] { 0, 5000 },
                    Rate = 0
                },
                new TaxBand
                {
                    Id = Guid.NewGuid(),
                    Name = "Tax Band B",
                    Range = new int[] { 5000, 20000 },
                    Rate = 20
                },
                new TaxBand
                {
                    Id = Guid.NewGuid(),
                    Name = "Tax Band C",
                    Range = new int[] { 20000, int.MaxValue },
                    Rate = 40
                }
            };

            _context.TaxBands.AddRange(taxBands);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} tax bands.", taxBands.Count);
        }
        else
        {
            _logger.LogInformation("Tax bands already exist. Skipping seeding.");
        }
    }

    public async Task SeedEmployeesAsync()
    {
        _logger.LogInformation("Seeding employees if none exist.");
        if (!_context.Employees.Any())
        {
            var employees = new List<Employee>
            {
                new Employee
                {
                    Id = Guid.NewGuid(),
                    FirstName = "John",
                    LastName = "Smith",
                    Salary = 10000,
                },
                new Employee
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Sarah",
                    LastName = "Johnson",
                    Salary = 20000,
                },
                new Employee
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Michael",
                    LastName = "Brown",
                    Salary = 75000,
                },
                new Employee
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Emily",
                    LastName = "Davis",
                    Salary = 95000,
                },
                new Employee
                {
                    Id = Guid.NewGuid(),
                    FirstName = "David",
                    LastName = "Wilson",
                    Salary = 120000,
                }
            };

            _context.Employees.AddRange(employees);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Seeded {Count} employees.", employees.Count);
        }
        else
        {
            _logger.LogInformation("Employees already exist. Skipping seeding.");
        }
    }
} 
