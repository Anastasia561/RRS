using System.Text.Json;

namespace RRS.Services;

public class ExchangeRateService : IExchangeRateService
{
    private readonly HttpClient _httpClient;

    public ExchangeRateService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<decimal> GetExchangeRateAsync(string targetCurrency)
    {
        if (string.Equals(targetCurrency, "PLN", StringComparison.OrdinalIgnoreCase))
            return 1.0m;

        var url = $"https://api.frankfurter.app/latest?from=PLN&to={targetCurrency.ToUpper()}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        if (doc.RootElement.TryGetProperty("rates", out var rates) &&
            rates.TryGetProperty(targetCurrency.ToUpper(), out var rateElem) &&
            rateElem.TryGetDecimal(out var rate))
        {
            return rate;
        }

        throw new Exception("Exchange rate not found.");
    }
}