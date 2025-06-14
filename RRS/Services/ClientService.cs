using Microsoft.EntityFrameworkCore;
using RRS.Data;
using RRS.Dtos;
using RRS.Models;

namespace RRS.Services;

public class ClientService : IClientService
{
    private readonly DatabaseContext _context;

    public ClientService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<List<IndividualDto>> GetAllIndividualsAsync(CancellationToken cancellationToken)
    {
        return await _context.Individuals.Where(i => i.IsRemoved == 0).Select(i => new IndividualDto()
        {
            FirstName = i.FirstName,
            LastName = i.LastName,
            Address = i.IdNavigation.Address,
            Email = i.IdNavigation.Email,
            Phone = i.IdNavigation.Phone,
            Pesel = i.Pesel,
        }).ToListAsync(cancellationToken);
    }
}