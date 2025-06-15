using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RRS.Services;

namespace RRS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SoftwaresController : ControllerBase
{
    private readonly ISoftwareService _softwareService;

    public SoftwaresController(ISoftwareService softwareService)
    {
        _softwareService = softwareService;
    }

    [Authorize(Roles = "User")]
    [HttpGet("all")]
    public async Task<IActionResult> GetSoftwareAsync(CancellationToken cancellationToken)
    {
        return Ok(await _softwareService.GetSoftwaresAsync(cancellationToken));
    }
}