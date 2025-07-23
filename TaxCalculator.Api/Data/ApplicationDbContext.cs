using Microsoft.EntityFrameworkCore;
using TaxCalculator.Api.Models;

namespace TaxCalculator.Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public virtual DbSet<Employee> Employees { get; set; }
    public virtual DbSet<TaxBand> TaxBands { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>()
            .Property(e => e.FullName)
            .HasComputedColumnSql("[FirstName] || ' ' || [LastName]", stored: true);
        
         modelBuilder.Entity<Employee>()
            .HasIndex(e => e.FullName)
            .HasDatabaseName("IX_Employee_FullName");
    }
}
