using RRS.Dtos;

namespace RRS.Services;

public interface IContractService
{
    public Task<List<ContractDto>> GetAllForClientAsync(int clientId, CancellationToken cancellationToken);
    public Task CreateContractAsync(int clientId, ContractCreateDto dto, CancellationToken cancellationToken);
    public Task PayForContractAsync(PaymentDto dto, int contractId, CancellationToken cancellationToken);
    public Task DeletePastDueContractsAsync(CancellationToken cancellationToken);
    public Task MarkContractsAsSignedAsync(CancellationToken cancellationToken);
}