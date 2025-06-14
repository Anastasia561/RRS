using Microsoft.EntityFrameworkCore;
using RRS.Data;
using RRS.Dtos;
using RRS.Exceptions;
using RRS.Models;

namespace RRS.Services;

public class CompanyService :ICompanyService
{
    private readonly DatabaseContext _context;

    public CompanyService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<List<CompanyDto>> GetAllCompaniesAsync(CancellationToken cancellationToken)
    {
        return await _context.Companies.Select(i => new CompanyDto()
        {
            Id = i.Id,
            Name = i.Name,
            Address = i.IdNavigation.Address,
            Email = i.IdNavigation.Email,
            Phone = i.IdNavigation.Phone,
            Krs = i.Krs,
        }).ToListAsync(cancellationToken);
    }

    public async Task<CompanyDto> GetCompanyByIdAsync(int id, CancellationToken cancellationToken)
    {
        var dto = await _context.Companies
            .Where(i => i.Id == id)
            .Select(i => new CompanyDto
            {
                Id = i.Id,
                Name = i.Name,
                Address = i.IdNavigation.Address,
                Email = i.IdNavigation.Email,
                Phone = i.IdNavigation.Phone,
                Krs = i.Krs,
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (dto == null)
        {
            throw new CompanyNotFoundException(id);
        }

        return dto;
    }

    public async Task UpdateCompanyAsync(int id, CompanyUpdateDto dto, CancellationToken cancellationToken)
    {
        var entity = await _context.Companies
            .Include(i => i.IdNavigation)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

        if (entity == null)
        {
            throw new CompanyNotFoundException(id);
        }

        entity.Name = dto.Name;
        entity.IdNavigation.Address = dto.Address;
        entity.IdNavigation.Phone = dto.Phone;
        entity.IdNavigation.Email = dto.Email;

        _context.Companies.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task CreateCompanyAsync(CompanyCreateDto dto, CancellationToken cancellationToken)
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

            var company = new Company
            {
                Id = client.Id,
                Name = dto.Name,
                Krs = dto.Krs
            };

            await _context.Companies.AddAsync(company, cancellationToken);
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