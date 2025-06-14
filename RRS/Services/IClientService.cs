using RRS.Dtos;

namespace RRS.Services;

public interface IClientService
{
    public Task<List<IndividualDto>> GetAllIndividualsAsync(CancellationToken cancellationToken);
}