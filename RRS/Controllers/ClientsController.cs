using Microsoft.AspNetCore.Mvc;
using RRS.Services;

namespace RRS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpGet("individuals/all")]
    public async Task<IActionResult> GetAllIndividuals(CancellationToken cancellationToken)
    {
        var result = await _clientService.GetAllIndividualsAsync(cancellationToken);
        return Ok(result);
    }
}