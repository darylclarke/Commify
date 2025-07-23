namespace TaxCalculator.Api.Models;

public class Employee
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty; // Computed column
    public int Salary { get; set; }
}
