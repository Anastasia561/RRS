using Microsoft.EntityFrameworkCore;
using RRS.Data;
using RRS.Dtos;
using RRS.Exceptions;
using RRS.Models;
using RRS.Services;

namespace RRS_Tests.Services;

public class CompanyServiceTests
{
    private readonly DatabaseContext _context;
    private readonly CompanyService _service;

    public CompanyServiceTests()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DatabaseContext(options);
        _service = new CompanyService(_context);
        SeedData();
    }

    private void SeedData()
    {
        var client = new Client
        {
            Address = "Test St.",
            Email = "test@example.com",
            Phone = "123456789"
        };

        var company = new Company
        {
            Name = "Test Company",
            Krs = "123456",
            IdNavigation = client
        };

        _context.Clients.Add(client);
        _context.Companies.Add(company);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllCompaniesAsync_ReturnsCompanyList()
    {
        var result = await _service.GetAllCompaniesAsync(CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Test Company", result[0].Name);
    }

    [Fact]
    public async Task GetCompanyByIdAsync_ValidId_ReturnsCompany()
    {
        var result = await _service.GetCompanyByIdAsync(1, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Test Company", result.Name);
    }

    [Fact]
    public async Task GetCompanyByIdAsync_InvalidId_ThrowsException()
    {
        await Assert.ThrowsAsync<CompanyNotFoundException>(() =>
            _service.GetCompanyByIdAsync(999, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateCompanyAsync_UpdatesDataSuccessfully()
    {
        var updateDto = new CompanyUpdateDto
        {
            Name = "Updated Name",
            Address = "New Address",
            Email = "new@example.com",
            Phone = "987654321"
        };

        await _service.UpdateCompanyAsync(1, updateDto, CancellationToken.None);

        var updated = await _context.Companies.Include(c => c.IdNavigation).FirstOrDefaultAsync(c => c.Id == 1);
        Assert.Equal("Updated Name", updated!.Name);
        Assert.Equal("New Address", updated.IdNavigation.Address);
    }
}