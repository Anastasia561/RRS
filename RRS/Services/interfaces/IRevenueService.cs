namespace RRS.Services;

public interface IRevenueService
{
    public Task<decimal> GetTotalRevenueForSoftwareAndCurrencyAsync(int? softwareId, string? currency,
        CancellationToken cancellationToken);

    public Task<decimal> GetPredictedRevenueForSoftwareAndCurrencyAsync(int? softwareId, string? currency,
        CancellationToken cancellationToken);
}