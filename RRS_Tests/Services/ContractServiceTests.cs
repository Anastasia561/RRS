using Microsoft.EntityFrameworkCore;
using RRS.Data;
using RRS.Dtos;
using RRS.Exceptions;
using RRS.Models;
using RRS.Services;

namespace RRS_Tests.Services;

public class ContractServiceTests
{
    private readonly DatabaseContext _context;
    private readonly ContractService _service;

    public ContractServiceTests()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new DatabaseContext(options);
        _service = new ContractService(_context);

        SeedData();
    }

    private void SeedData()
    {
        var client = new Client { Address = "address", Email = "email@email.com", Phone = "12345678" };
        _context.Clients.Add(client);

        var software = new Software
        {
            Id = 1,
            Name = "Test Software",
            Description = "Test Software Description",
            YearCost = 3660m,
            Discounts = new List<Discount>
            {
                new Discount
                {
                    Name = "Discount", StartDate = DateTime.Today.AddDays(-10), EndDate = DateTime.Today.AddDays(10),
                    Value = 10
                }
            },
            Category = new Category { Name = "Category1" }
        };
        _context.Softwares.Add(software);

        var version = new RRS.Models.Version() { Id = 1, Number = 1, ReleaseDate = DateTime.Today.AddMonths(-1) };
        _context.Versions.Add(version);
        _context.Updates.AddRange(new Update { Id = 1, Name = "Update1" }, new Update { Id = 2, Name = "Update2" });
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllForClientAsync_ClientExists_ReturnsContracts()
    {
        var contract = new Contract
        {
            ClientId = 1,
            SoftwareId = 1,
            VersionId = 1,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(30),
            FinalPrice = 100m,
            IsSigned = 1,
            UpdateSupport = 1,
            Software = _context.Softwares.First(),
            Version = _context.Versions.First(),
            Updates = new List<Update>()
        };
        _context.Contracts.Add(contract);
        await _context.SaveChangesAsync();

        var contracts = await _service.GetAllForClientAsync(1, CancellationToken.None);

        Assert.Single(contracts);
        Assert.Equal(contract.Id, contracts[0].Id);
        Assert.Equal("Test Software", contracts[0].Software.Name);
    }

    [Fact]
    public async Task GetAllForClientAsync_ClientDoesNotExist_ThrowsClientNotFoundException()
    {
        await Assert.ThrowsAsync<ClientNotFoundException>(() =>
            _service.GetAllForClientAsync(999, CancellationToken.None));
    }

    [Fact]
    public async Task CreateContractAsync_ClientNotFound_ThrowsException()
    {
        var dto = new ContractCreateDto
        {
            SoftwareId = 1, SoftwareVersionId = 1, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(30),
            UpdatesSupportYears = 1, Updates = new List<string>()
        };
        await Assert.ThrowsAsync<ClientNotFoundException>(() =>
            _service.CreateContractAsync(999, dto, CancellationToken.None));
    }

    [Fact]
    public async Task CreateContractAsync_SoftwareNotFound_ThrowsException()
    {
        var dto = new ContractCreateDto
        {
            SoftwareId = 999, SoftwareVersionId = 1, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(30),
            UpdatesSupportYears = 1, Updates = new List<string>()
        };
        await Assert.ThrowsAsync<SoftwareNotFoundException>(() =>
            _service.CreateContractAsync(1, dto, CancellationToken.None));
    }

    [Fact]
    public async Task CreateContractAsync_VersionNotFound_ThrowsException()
    {
        var dto = new ContractCreateDto
        {
            SoftwareId = 1, SoftwareVersionId = 999, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(30),
            UpdatesSupportYears = 1, Updates = new List<string>()
        };
        await Assert.ThrowsAsync<VersionNotFoundException>(() =>
            _service.CreateContractAsync(1, dto, CancellationToken.None));
    }

    [Fact]
    public async Task CreateContractAsync_UpdateNotFound_ThrowsException()
    {
        var dto = new ContractCreateDto
        {
            SoftwareId = 1,
            SoftwareVersionId = 1,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(30),
            UpdatesSupportYears = 1,
            Updates = new List<string> { "NonExistingUpdate" }
        };
        await Assert.ThrowsAsync<UpdateNotFoundException>(() =>
            _service.CreateContractAsync(1, dto, CancellationToken.None));
    }

    [Fact]
    public async Task PayForContractAsync_ValidPayment_AddsPayment()
    {
        var contract = new Contract
        {
            Id = 1,
            ClientId = 1,
            SoftwareId = 1,
            VersionId = 1,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(30),
            FinalPrice = 100m,
            IsSigned = 0,
            Payments = new List<Payment>()
        };
        _context.Contracts.Add(contract);
        await _context.SaveChangesAsync();

        var paymentDto = new PaymentDto { Amount = 50m };

        await _service.PayForContractAsync(paymentDto, 1, CancellationToken.None);

        var payment = _context.Payments.FirstOrDefault(p => p.ContractId == 1);
        Assert.NotNull(payment);
        Assert.Equal(50m, payment.Amount);
    }

    [Fact]
    public async Task PayForContractAsync_ContractNotFound_ThrowsException()
    {
        var paymentDto = new PaymentDto { Amount = 50m };
        await Assert.ThrowsAsync<ContractException>(() =>
            _service.PayForContractAsync(paymentDto, 999, CancellationToken.None));
    }

    [Fact]
    public async Task DeletePastDueContractsAsync_RemovesExpiredUnsignedContracts()
    {
        var contract = new Contract
        {
            Id = 1,
            ClientId = 1,
            SoftwareId = 1,
            VersionId = 1,
            StartDate = DateTime.Today.AddDays(-40),
            EndDate = DateTime.Today.AddDays(-10),
            FinalPrice = 100m,
            IsSigned = 0,
            Payments = new List<Payment> { new Payment { Amount = 10m } },
            Updates = new List<Update> { new Update { Name = "Update1" } }
        };
        _context.Contracts.Add(contract);
        await _context.SaveChangesAsync();

        await _service.DeletePastDueContractsAsync(CancellationToken.None);

        Assert.Empty(_context.Contracts.ToList());
        Assert.Empty(_context.Payments.ToList());
    }

    [Fact]
    public async Task MarkContractsAsSignedAsync_SignsFullyPaidContracts()
    {
        var contract = new Contract
        {
            Id = 1,
            ClientId = 1,
            SoftwareId = 1,
            VersionId = 1,
            StartDate = DateTime.Today.AddDays(-10),
            EndDate = DateTime.Today.AddDays(10),
            FinalPrice = 100m,
            IsSigned = 0,
            Payments = new List<Payment> { new Payment { Amount = 100m } }
        };
        _context.Contracts.Add(contract);
        await _context.SaveChangesAsync();

        await _service.MarkContractsAsSignedAsync(CancellationToken.None);

        var signedContract = await _context.Contracts.FindAsync(1);
        Assert.Equal(1, signedContract.IsSigned);
    }
}