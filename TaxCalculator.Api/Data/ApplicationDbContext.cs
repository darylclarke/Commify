using Microsoft.EntityFrameworkCore;
using TaxCalculator.Api.Models;

namespace TaxCalculator.Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public virtual DbSet<Employee> Employees { get; set; }
    public virtual DbSet<TaxBand> TaxBands { get; set; }
}
