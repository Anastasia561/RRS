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

    public async Task CreateContract(int clientId, ContractCreateDto dto, CancellationToken cancellationToken)
    {
        if (!_context.Clients.Any(c => c.Id == clientId))
            throw new ClientNotFoundException(clientId);

        if (!_context.Softwares.Any(s => s.Id == dto.SoftwareId))
            throw new SoftwareNotFoundException(dto.SoftwareId);

        if (!_context.Versions.Any(v => v.Id == dto.SoftwareVersionId))
            throw new VersionNotFoundException(dto.SoftwareVersionId);

        if (_context.Contracts.Any(c => c.SoftwareId == dto.SoftwareId
                                        && c.Version.Number == dto.SoftwareVersionId && c.EndDate > dto.StartDate))
            throw new ContractIsActiveException(clientId);

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
}