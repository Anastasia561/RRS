using Microsoft.EntityFrameworkCore;
using RRS.Data;
using RRS.Models;
using RRS.Services;

namespace RRS_Tests.Services;

public class SoftwareServiceTests
{
    private readonly DatabaseContext _context;
    private readonly SoftwareService _service;

    public SoftwareServiceTests()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DatabaseContext(options);
        SeedData().Wait();

        _service = new SoftwareService(_context);
    }

    private async Task SeedData()
    {
        var category = new Category { Id = 1, Name = "CRM" };
        var version = new RRS.Models.Version() { Id = 1, Number = 1, ReleaseDate = new DateTime(2023, 1, 1) };
        var software = new Software
        {
            Id = 1,
            Name = "MySoftware",
            Description = "Test software",
            YearCost = 1200,
            Category = category,
            CategoryId = 1,
            CurrentVersion = version,
            CurrentVersionId = 1,
            Discounts = new List<Discount>
            {
                new Discount
                {
                    Id = 1,
                    Name = "New Year",
                    Value = 10,
                    StartDate = DateTime.Now.AddDays(-10),
                    EndDate = DateTime.Now.AddDays(10),
                    IsUpfront = 1
                },
                new Discount
                {
                    Id = 2,
                    Name = "Expired Discount",
                    Value = 5,
                    StartDate = DateTime.Now.AddDays(-30),
                    EndDate = DateTime.Now.AddDays(-1),
                    IsUpfront = 1
                }
            }
        };

        await _context.Categories.AddAsync(category);
        await _context.Versions.AddAsync(version);
        await _context.Softwares.AddAsync(software);
        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetSoftwaresAsync_ReturnsSoftwareWithCorrectData()
    {
        var result = await _service.GetSoftwaresAsync(CancellationToken.None);

        Assert.Single(result);

        var software = result.First();
        Assert.Equal("MySoftware", software.Name);
        Assert.Equal("Test software", software.Description);
        Assert.Equal("CRM", software.Category);
        Assert.Equal(1, software.CurrentVersion.Number);
        Assert.Equal(1, software.Discounts.Count);
        Assert.Equal("New Year", software.Discounts.First().Name);
    }

    [Fact]
    public async Task GetSoftwaresAsync_EmptyDatabase_ReturnsEmptyList()
    {
        var emptyOptions = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var emptyContext = new DatabaseContext(emptyOptions);
        var emptyService = new SoftwareService(emptyContext);


        var result = await emptyService.GetSoftwaresAsync(CancellationToken.None);

        Assert.Empty(result);
    }
}