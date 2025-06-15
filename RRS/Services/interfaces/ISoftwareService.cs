using RRS.Dtos;

namespace RRS.Services;

public interface ISoftwareService
{
    public Task<List<SoftwareDto>> GetSoftwaresAsync(CancellationToken cancellationToken);
}