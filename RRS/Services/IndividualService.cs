using Microsoft.EntityFrameworkCore;
using RRS.Data;
using RRS.Dtos;
using RRS.Exceptions;
using RRS.Models;

namespace RRS.Services;

public class IndividualService : IIndividualService
{
    private readonly DatabaseContext _context;

    public IndividualService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<List<IndividualDto>> GetAllIndividualsAsync(CancellationToken cancellationToken)
    {
        return await _context.Individuals.Where(i => i.IsRemoved == 0).Select(i => new IndividualDto()
        {
            Id = i.Id,
            FirstName = i.FirstName,
            LastName = i.LastName,
            Address = i.IdNavigation.Address,
            Email = i.IdNavigation.Email,
            Phone = i.IdNavigation.Phone,
            Pesel = i.Pesel,
        }).ToListAsync(cancellationToken);
    }

    public async Task<IndividualDto> GetIndividualByIdAsync(int id, CancellationToken cancellationToken)
    {
        var dto = await _context.Individuals
            .Where(i => i.IsRemoved == 0 && i.Id == id)
            .Select(i => new IndividualDto
            {
                Id = i.Id,
                FirstName = i.FirstName,
                LastName = i.LastName,
                Address = i.IdNavigation.Address,
                Email = i.IdNavigation.Email,
                Phone = i.IdNavigation.Phone,
                Pesel = i.Pesel,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (dto == null)
        {
            throw new IndividualNotFoundException(id);
        }

        return dto;
    }

    public async Task UpdateIndividualAsync(int id, IndividualUpdateDto dto, CancellationToken cancellationToken)
    {
        var entity = await _context.Individuals
            .Include(i => i.IdNavigation)
            .FirstOrDefaultAsync(i => i.Id == id && i.IsRemoved == 0, cancellationToken);

        if (entity == null)
        {
            throw new IndividualNotFoundException(id);
        }

        entity.FirstName = dto.FirstName;
        entity.LastName = dto.LastName;
        entity.IdNavigation.Address = dto.Address;
        entity.IdNavigation.Phone = dto.Phone;
        entity.IdNavigation.Email = dto.Email;

        _context.Individuals.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteIndividualAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _context.Individuals
            .FirstOrDefaultAsync(i => i.Id == id && i.IsRemoved == 0, cancellationToken);
        if (entity == null)
        {
            throw new IndividualNotFoundException(id);
        }

        entity.IsRemoved = 1;
        _context.Individuals.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task CreateIndividualAsync(IndividualDto dto, CancellationToken cancellationToken)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var client = new Client
            {
                Address = dto.Address,
                Email = dto.Email,
                Phone = dto.Phone
            };

            await _context.Clients.AddAsync(client, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            var individual = new Individual
            {
                Id = client.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Pesel = dto.Pesel,
                IsRemoved = 0
            };

            await _context.Individuals.AddAsync(individual, cancellationToken);
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