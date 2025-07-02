namespace TaxCalculator.Api.Models;

public class TaxBand
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int[] Range { get; set; } = [];
    public int Rate { get; set; }
}
