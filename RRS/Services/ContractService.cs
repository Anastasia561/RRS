using Microsoft.EntityFrameworkCore;
using RRS.Data;
using RRS.Dtos;
using RRS.Exceptions;
using RRS.Models;

namespace RRS.Services;

public class ContractService : IContractService
{
    private readonly DatabaseContext _context;

    public ContractService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<List<ContractDto>> GetAllForClientAsync(int clientId, CancellationToken cancellationToken)
    {
        if (!_context.Clients.Any(c => c.Id == clientId))
        {
            throw new ClientNotFoundException(clientId);
        }

        return await _context.Contracts.Where(c => c.ClientId == clientId).Select(c => new ContractDto
        {
            Id = c.Id,
            StartDate = c.StartDate,
            EndDate = c.EndDate,
            FinalPrice = c.FinalPrice,
            UpdatesSupportYears = c.UpdateSupport,
            IsSigned = c.IsSigned,
            Software = new SoftwareContractDto()
            {
                Id = c.SoftwareId,
                Name = c.Software.Name,
                Description = c.Software.Description,
                Category = c.Software.Category.Name,
            },
            Updates = c.Updates.Select(u => u.Name).ToList(),
            SoftwareVersion = new VersionDto()
            {
                Number = c.Version.Number,
                ReleaseDate = c.Version.ReleaseDate
            }
        }).ToListAsync(cancellationToken);
    }

    public async Task CreateContractAsync(int clientId, ContractCreateDto dto, CancellationToken cancellationToken)
    {
        if (!_context.Clients.Any(c => c.Id == clientId))
            throw new ClientNotFoundException(clientId);

        if (!_context.Softwares.Any(s => s.Id == dto.SoftwareId))
            throw new SoftwareNotFoundException(dto.SoftwareId);

        if (!_context.Versions.Any(v => v.Id == dto.SoftwareVersionId))
            throw new VersionNotFoundException(dto.SoftwareVersionId);

        if (_context.Contracts.Any(c => c.SoftwareId == dto.SoftwareId
                                        && c.Version.Number == dto.SoftwareVersionId && c.EndDate > dto.StartDate))
            throw new ContractException(
                $"Client with id {clientId} has already active contract for this version of software");

        foreach (var name in dto.Updates.Where(name => !_context.Updates.Any(u => u.Name == name)))
        {
            throw new UpdateNotFoundException(name);
        }

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var software = await _context.Softwares
                .Include(s => s.Discounts).Where(s => s.Id == dto.SoftwareId)
                .FirstOrDefaultAsync(cancellationToken);
            var totalPriceWithoutDiscounts = software.YearCost / 366 * (dto.EndDate - dto.StartDate).Days +
                                             dto.UpdatesSupportYears * 1000;
            var discount = software.Discounts
                .Where(d => d.EndDate > dto.StartDate && d.StartDate <= dto.StartDate)
                .OrderByDescending(d => d.Value)
                .FirstOrDefault()?.Value ?? 0m;

            if (_context.Contracts.Any(c => c.Client.Id == clientId && c.IsSigned == 1))
            {
                totalPriceWithoutDiscounts = totalPriceWithoutDiscounts * 0.95m;
            }

            var finalPrice = totalPriceWithoutDiscounts - totalPriceWithoutDiscounts * (discount / 100m);

            var updates = _context.Updates
                .Where(u => dto.Updates.Contains(u.Name))
                .ToList();

            var contract = new Contract
            {
                ClientId = clientId,
                SoftwareId = dto.SoftwareId,
                VersionId = dto.SoftwareVersionId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                UpdateSupport = dto.UpdatesSupportYears,
                IsSigned = 0,
                FinalPrice = finalPrice,
                Updates = updates
            };

            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task PayForContractAsync(PaymentDto dto, int contractId, CancellationToken cancellationToken)
    {
        if (!_context.Contracts.Any(c => c.Id == contractId))
            throw new ContractException($"Contract with {contractId} not found");

        if (_context.Contracts.Any(c => (c.Id == contractId && c.IsSigned == 1) ||
                                        (c.Id == contractId && c.EndDate < DateTime.Now)))
            throw new ContractException($"Contract with id {contractId} is no longer available for payment");

        if (_context.Contracts.Any(c => c.Id == contractId &&
                                        c.FinalPrice - c.Payments.Sum(p => p.Amount) < dto.Amount))
            throw new ContractException($"Amount payed for contract with id {contractId} is too big");


        _context.Payments.Add(new Payment()
        {
            Amount = dto.Amount,
            ContractId = contractId,
            Date = DateTime.Now
        });
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeletePastDueContractsAsync(CancellationToken cancellationToken)
    {
        var contractsToDelete = _context.Contracts
            .Include(c => c.Payments)
            .Include(c => c.Updates)
            .Where(c => c.EndDate < DateTime.Now && c.IsSigned == 0)
            .ToList();

        foreach (var contract in contractsToDelete)
        {
            _context.Payments.RemoveRange(contract.Payments);
            contract.Updates.Clear();
        }

        await _context.SaveChangesAsync(cancellationToken);
        _context.Contracts.RemoveRange(contractsToDelete);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkContractsAsSignedAsync(CancellationToken cancellationToken)
    {
        var contractsToSign = _context.Contracts
            .Include(c => c.Payments)
            .Where(c => c.EndDate >= DateTime.Now
                        && c.IsSigned == 0
                        && Math.Abs(c.Payments.Sum(p => p.Amount) - c.FinalPrice) < 0.01m)
            .ToList();

        foreach (var contract in contractsToSign)
        {
            contract.IsSigned = 1;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}