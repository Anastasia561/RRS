using Microsoft.EntityFrameworkCore;
using Moq;
using RRS.Data;
using RRS.Services;
using Contract = RRS.Models.Contract;

namespace RRS_Tests.Services;

public class RevenueServiceTests
{
    private readonly Mock<IExchangeRateService> _exchangeRateServiceMock;
    private readonly DatabaseContext _context;
    private readonly RevenueService _revenueService;

    public RevenueServiceTests()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DatabaseContext(options);

        _exchangeRateServiceMock = new Mock<IExchangeRateService>();

        _revenueService = new RevenueService(_context, _exchangeRateServiceMock.Object);

        SeedData();
    }

    private void SeedData()
    {
        _context.Contracts.AddRange(
            new Contract() { Id = 1, SoftwareId = 100, FinalPrice = 1000m, IsSigned = 1},
            new Contract() { Id = 2, SoftwareId = 100, FinalPrice = 2000m, IsSigned = 1 },
            new Contract() { Id = 3, SoftwareId = 200, FinalPrice = 3000m, IsSigned = 0 }
        );
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetTotalRevenueForSoftwareAndCurrencyAsync_ReturnsCorrectTotalInPLN()
    {
        var result =
            await _revenueService.GetTotalRevenueForSoftwareAndCurrencyAsync(100, "PLN", CancellationToken.None);
        Assert.Equal(3000m, result);
    }

    [Fact]
    public async Task GetTotalRevenueForSoftwareAndCurrencyAsync_ReturnsCorrectTotalInUSD()
    {
        _exchangeRateServiceMock.Setup(x => x.GetExchangeRateAsync("USD"))
            .ReturnsAsync(0.25m);

        var result =
            await _revenueService.GetTotalRevenueForSoftwareAndCurrencyAsync(100, "USD", CancellationToken.None);
        Assert.Equal(750m, result);
    }

    [Fact]
    public async Task GetTotalRevenueForSoftwareAndCurrencyAsync_UsesDefaultWhenCurrencyIsNull()
    {
        var result =
            await _revenueService.GetTotalRevenueForSoftwareAndCurrencyAsync(100, null, CancellationToken.None);
        Assert.Equal(3000m, result);
    }

    [Fact]
    public async Task GetTotalRevenueForSoftwareAndCurrencyAsync_ReturnsZeroIfNoContracts()
    {
        var result =
            await _revenueService.GetTotalRevenueForSoftwareAndCurrencyAsync(999, "PLN", CancellationToken.None);
        Assert.Equal(0m, result);
    }
}