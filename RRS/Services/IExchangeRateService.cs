namespace RRS.Services;

public interface IExchangeRateService
{
    public Task<decimal> GetExchangeRateAsync(string targetCurrency);
}