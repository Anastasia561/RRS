using Microsoft.EntityFrameworkCore;
using RRS.Data;
using RRS.Dtos;
using RRS.Exceptions;
using RRS.Models;
using RRS.Services;

namespace RRS_Tests.Services;

public class IndividualServiceTests
{
    private readonly DatabaseContext _context;
    private readonly IndividualService _service;

    public IndividualServiceTests()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new DatabaseContext(options);
        _service = new IndividualService(_context);

        SeedData();
    }

    private void SeedData()
    {
        var client = new Client { Id = 1, Address = "Address1", Email = "email1@test.com", Phone = "123456" };
        _context.Clients.Add(client);

        var individual = new Individual
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Pesel = "12345678901",
            IsRemoved = 0,
            IdNavigation = client
        };

        _context.Individuals.Add(individual);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllIndividualsAsync_ReturnsNotRemovedIndividuals()
    {
        var result = await _service.GetAllIndividualsAsync(CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("John", result[0].FirstName);
    }

    [Fact]
    public async Task GetIndividualByIdAsync_ExistingId_ReturnsIndividual()
    {
        var result = await _service.GetIndividualByIdAsync(1, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("John", result.FirstName);
    }

    [Fact]
    public async Task GetIndividualByIdAsync_NonExistingId_ThrowsIndividualNotFoundException()
    {
        await Assert.ThrowsAsync<IndividualNotFoundException>(async () =>
            await _service.GetIndividualByIdAsync(999, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateIndividualAsync_ExistingId_UpdatesIndividual()
    {
        var updateDto = new IndividualUpdateDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            Address = "New Address",
            Email = "newemail@test.com",
            Phone = "654321"
        };

        await _service.UpdateIndividualAsync(1, updateDto, CancellationToken.None);

        var updated = await _context.Individuals.Include(i => i.IdNavigation).FirstAsync(i => i.Id == 1);
        Assert.Equal("Jane", updated.FirstName);
        Assert.Equal("New Address", updated.IdNavigation.Address);
    }

    [Fact]
    public async Task UpdateIndividualAsync_NonExistingId_ThrowsIndividualNotFoundException()
    {
        var updateDto = new IndividualUpdateDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            Address = "New Address",
            Email = "newemail@test.com",
            Phone = "654321"
        };

        await Assert.ThrowsAsync<IndividualNotFoundException>(async () =>
            await _service.UpdateIndividualAsync(999, updateDto, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteIndividualAsync_SetsIsRemovedToOne()
    {
        await _service.DeleteIndividualAsync(1, CancellationToken.None);

        var entity = await _context.Individuals.FindAsync(1);
        Assert.Equal(1, entity.IsRemoved);
    }

    [Fact]
    public async Task DeleteIndividualAsync_NonExistingId_ThrowsIndividualNotFoundException()
    {
        await Assert.ThrowsAsync<IndividualNotFoundException>(async () =>
            await _service.DeleteIndividualAsync(999, CancellationToken.None));
    }
}