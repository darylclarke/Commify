using TaxCalculator.Api.Models;

namespace TaxCalculator.Api.Services;

public interface ITaxCalculationService
{
    Task<TaxCalculationResult> CalculateTaxAsync(int grossAnnualSalary, CancellationToken cancellationToken);
}
