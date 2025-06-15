using Microsoft.EntityFrameworkCore;
using RRS.Data;

namespace RRS.Services;

public class RevenueService : IRevenueService
{
    private readonly DatabaseContext _context;
    private readonly IExchangeRateService _exchangeRateService;

    public RevenueService(DatabaseContext context, IExchangeRateService exchangeRateService)
    {
        _context = context;
        _exchangeRateService = exchangeRateService;
    }

    public async Task<decimal> GetTotalRevenueForSoftwareAndCurrencyAsync(int? softwareId, string? currency,
        CancellationToken cancellationToken)
    {
        var query = _context.Contracts.Where(c => c.IsSigned == 1).AsQueryable();

        if (softwareId.HasValue)
            query = query.Where(c => c.SoftwareId == softwareId.Value);

        var totalPln = await query.SumAsync(c => c.FinalPrice, cancellationToken);

        if (string.IsNullOrEmpty(currency) || currency.ToUpper() == "PLN")
            return totalPln;
        var rate = await _exchangeRateService.GetExchangeRateAsync(currency);
        return totalPln * rate;
    }

    public async Task<decimal> GetPredictedRevenueForSoftwareAndCurrencyAsync(int? softwareId, string? currency,
        CancellationToken cancellationToken)
    {
        var query = _context.Contracts.AsQueryable();

        if (softwareId.HasValue)
            query = query.Where(c => c.SoftwareId == softwareId.Value);

        var totalPln = await query.SumAsync(c => c.FinalPrice, cancellationToken);

        if (string.IsNullOrEmpty(currency) || currency.ToUpper() == "PLN")
            return totalPln;
        var rate = await _exchangeRateService.GetExchangeRateAsync(currency);
        return totalPln * rate;
    }
}