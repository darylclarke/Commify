using FluentAssertions;
using TaxCalculator.Api.Services;
using Xunit;
using TaxCalculator.Api.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using TaxCalculator.Api.Data;

namespace TaxCalculator.Tests
{
    public class TaxCalculationServiceTests
    {
        private readonly ITaxCalculationService _taxService;
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _memoryCache;
        private readonly IOptions<CacheSettings> _cacheOptions;

        public TaxCalculationServiceTests()
        {
            // Setup in-memory DB context
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            _context = new ApplicationDbContext(options);

            if (!_context.TaxBands.Any())
            {
                _context.TaxBands.AddRange(new List<TaxBand>
                {
                    new TaxBand { Id = Guid.NewGuid(), Name = "Band A", Range = new[] { 0, 5000 }, Rate = 0 },
                    new TaxBand { Id = Guid.NewGuid(), Name = "Band B", Range = new[] { 5000, 20000 }, Rate = 20 },
                    new TaxBand { Id = Guid.NewGuid(), Name = "Band C", Range = new[] { 20000, int.MaxValue }, Rate = 40 },
                });
                _context.SaveChanges();
            }

            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _cacheOptions = Options.Create(new CacheSettings { TaxBandsDurationHours = 12 });
            
            _taxService = new TaxCalculationService(_context, _memoryCache, _cacheOptions, NullLogger<TaxCalculationService>.Instance);
        }

        [Fact]
        public async Task CalculateTax_Example_10000_ShouldMatchManualCalculation()
        {
            // Act
            var result = await _taxService.CalculateTaxAsync(10000, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.GrossAnnualSalary.Should().Be(10000);
            result.GrossMonthlySalary.Should().BeApproximately(833.33m, 0.01m);
            result.AnnualTaxPaid.Should().Be(1000); 
            result.MonthlyTaxPaid.Should().BeApproximately(83.33m, 0.01m);
            result.NetAnnualSalary.Should().Be(9000);
            result.NetMonthlySalary.Should().BeApproximately(750m, 0.01m);
        }

        [Fact]
        public async Task CalculateTax_Example_40000_ShouldMatchManualCalculation()
        {
            // Act
            var result = await _taxService.CalculateTaxAsync(40000, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.GrossAnnualSalary.Should().Be(40000);
            result.GrossMonthlySalary.Should().BeApproximately(3333.33m, 0.01m);
            result.AnnualTaxPaid.Should().Be(11000); 
            result.MonthlyTaxPaid.Should().BeApproximately(916.67m, 0.01m);
            result.NetAnnualSalary.Should().Be(29000);
            result.NetMonthlySalary.Should().BeApproximately(2416.67m, 0.01m);
        }

        [Fact]
        public async Task CalculateTax_WithNegativeSalary_ShouldReturnException()
        {
            // Act & Assert
            await FluentActions
                .Awaiting(() => _taxService.CalculateTaxAsync(-1000, CancellationToken.None))
                .Should().ThrowAsync<InvalidSalaryException>();
        }

        [Fact]
        public async Task CalculateTax_Example_5000_ShouldBeAllBandA()
        {
            // Act
            var result = await _taxService.CalculateTaxAsync(5000, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.GrossAnnualSalary.Should().Be(5000);
            result.GrossMonthlySalary.Should().BeApproximately(416.67m, 0.01m);
            result.AnnualTaxPaid.Should().Be(0); 
            result.MonthlyTaxPaid.Should().Be(0);
            result.NetAnnualSalary.Should().Be(5000);
            result.NetMonthlySalary.Should().BeApproximately(416.67m, 0.01m);
        }

        [Fact]
        public async Task CalculateTax_Example_20000_ShouldBeBandAandB()
        {
            // Act
            var result = await _taxService.CalculateTaxAsync(20000, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.GrossAnnualSalary.Should().Be(20000);
            result.GrossMonthlySalary.Should().BeApproximately(1666.67m, 0.01m);
            result.AnnualTaxPaid.Should().Be(3000);
            result.MonthlyTaxPaid.Should().BeApproximately(250m, 0.01m);
            result.NetAnnualSalary.Should().Be(17000);
            result.NetMonthlySalary.Should().BeApproximately(1416.67m, 0.01m);
        }
    }
}
