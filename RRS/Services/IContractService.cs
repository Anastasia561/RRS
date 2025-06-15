using RRS.Dtos;

namespace RRS.Services;

public interface IContractService
{
    public Task<List<ContractDto>> GetAllForClientAsync(int clientId, CancellationToken cancellationToken);
    public Task CreateContract(int clientId, ContractCreateDto dto, CancellationToken cancellationToken);
}