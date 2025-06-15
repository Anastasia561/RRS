using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RRS.Exceptions;
using RRS.Services;

namespace RRS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RevenuesController : ControllerBase
{
    private readonly IRevenueService _revenueService;

    public RevenuesController(IRevenueService revenueService)
    {
        _revenueService = revenueService;
    }

    [Authorize(Roles = "User")]
    [HttpGet("total")]
    public async Task<IActionResult> GetTotalRevenueAsync([FromQuery] int? softwareId,
        [FromQuery] string? currency,
        CancellationToken cancellationToken)
    {
        try
        {
            var result =
                await _revenueService.GetTotalRevenueForSoftwareAndCurrencyAsync(softwareId,
                    currency, cancellationToken);
            return Ok(result);
        }
        catch (SoftwareNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [Authorize(Roles = "User")]
    [HttpGet("predicted")]
    public async Task<IActionResult> GetPredictedRevenueAsync([FromQuery] int? softwareId,
        [FromQuery] string? currency,
        CancellationToken cancellationToken)
    {
        try
        {
            var result =
                await _revenueService.GetPredictedRevenueForSoftwareAndCurrencyAsync(softwareId,
                    currency, cancellationToken);
            return Ok(result);
        }
        catch (SoftwareNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }
}