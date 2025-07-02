using TaxCalculator.Api.Models;
using Microsoft.EntityFrameworkCore;
using TaxCalculator.Api.Data;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace TaxCalculator.Api.Services;

public class TaxCalculationService : ITaxCalculationService
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private const string TaxBandsCacheKey = "TaxBands";
    private readonly CacheSettings _cacheSettings;
    private readonly ILogger<TaxCalculationService> _logger;

    public TaxCalculationService(ApplicationDbContext context, IMemoryCache cache, IOptions<CacheSettings> cacheOptions, ILogger<TaxCalculationService> logger)
    {
        _context = context;
        _cache = cache;
        _cacheSettings = cacheOptions.Value;
        _logger = logger;
    }

    private async Task<List<TaxBand>> GetTaxBandsAsync(CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(TaxBandsCacheKey, out List<TaxBand> cachedBands))
        {
            _logger.LogDebug("Tax bands cache hit.");
            return cachedBands;
        }
        _logger.LogDebug("Tax bands cache miss. Loading from database.");
        var bands = await _context.TaxBands.OrderBy(tb => tb.Range[0]).AsNoTracking().ToListAsync(cancellationToken);
        _cache.Set(TaxBandsCacheKey, bands, TimeSpan.FromHours(_cacheSettings.TaxBandsDurationHours));
        _logger.LogDebug("Cached {Count} tax bands.", bands.Count);
        return bands;
    }

    public async Task<TaxCalculationResult> CalculateTaxAsync(int grossAnnualSalary, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Calculating tax for salary {Salary}", grossAnnualSalary);

        if (grossAnnualSalary < 0)
        {
            _logger.LogWarning("Invalid salary: {Salary}", grossAnnualSalary);
            throw new InvalidSalaryException("Salary must be non-negative.");
        }

        var taxBands = await GetTaxBandsAsync(cancellationToken);
        _logger.LogDebug("Retrieved {Count} tax bands for calculation.", taxBands.Count);
        decimal totalTax = 0;

        foreach (var band in taxBands)
        {
            if (grossAnnualSalary > band.Range[0])
            {
                var taxableAmount = Math.Min(grossAnnualSalary - band.Range[0], 
                    band.Range[1] - band.Range[0]);
                totalTax += taxableAmount * (band.Rate / 100m);
            }
        }

        var grossAnnual = (decimal)grossAnnualSalary;
        var grossMonthly = grossAnnual / 12;
        var netAnnual = grossAnnual - totalTax;
        var netMonthly = netAnnual / 12;

        _logger.LogInformation("Tax calculation complete for salary {Salary}: AnnualTaxPaid={AnnualTaxPaid}, NetAnnualSalary={NetAnnualSalary}", grossAnnualSalary, totalTax, netAnnual);

        return new TaxCalculationResult
        {
            GrossAnnualSalary = grossAnnual,
            GrossMonthlySalary = grossMonthly,
            NetAnnualSalary = netAnnual,
            NetMonthlySalary = netMonthly,
            AnnualTaxPaid = totalTax,
            MonthlyTaxPaid = totalTax / 12
        };
    }
} 
