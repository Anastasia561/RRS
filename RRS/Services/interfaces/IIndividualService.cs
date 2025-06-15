using RRS.Dtos;

namespace RRS.Services;

public interface IIndividualService
{
    public Task<List<IndividualDto>> GetAllIndividualsAsync(CancellationToken cancellationToken);
    public Task<IndividualDto> GetIndividualByIdAsync(int id, CancellationToken cancellationToken);
    public Task UpdateIndividualAsync(int id, IndividualUpdateDto dto, CancellationToken cancellationToken);
    public Task DeleteIndividualAsync(int id, CancellationToken cancellationToken);
    public Task CreateIndividualAsync(IndividualCreateDto dto, CancellationToken cancellationToken);
}