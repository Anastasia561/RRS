using Microsoft.EntityFrameworkCore;
using RRS.Data;
using RRS.Dtos;

namespace RRS.Services;

public class SoftwareService : ISoftwareService
{
    private readonly DatabaseContext _context;

    public SoftwareService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<List<SoftwareDto>> GetSoftwaresAsync(CancellationToken cancellationToken)
    {
        return await _context.Softwares.Select(s => new SoftwareDto()
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description,
            Category = s.Category.Name,
            YearCost = s.YearCost,
            CurrentVersion = new VersionDto()
            {
                Number = s.CurrentVersion.Number,
                ReleaseDate = s.CurrentVersion.ReleaseDate
            },
            Discounts = s.Discounts.Where(d => d.EndDate > DateTime.Now).Select(d => new DiscountDto()
            {
                Name = d.Name,
                Amount = d.Value,
                EndDate = d.EndDate,
                StartDate = d.StartDate
            }).ToList()
        }).ToListAsync(cancellationToken);
    }
}