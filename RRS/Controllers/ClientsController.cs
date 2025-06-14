using Microsoft.AspNetCore.Mvc;
using RRS.Dtos;
using RRS.Exceptions;
using RRS.Services;

namespace RRS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IIndividualService _individualService;

    public ClientsController(IIndividualService individualService)
    {
        this._individualService = individualService;
    }

    [HttpGet("individuals/all")]
    public async Task<IActionResult> GetAllIndividualsAsync(CancellationToken cancellationToken)
    {
        var result = await _individualService.GetAllIndividualsAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("individuals/{id}")]
    public async Task<IActionResult> GetIndividualByIdAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _individualService.GetIndividualByIdAsync(id, cancellationToken));
        }
        catch (IndividualNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost("individuals/{id}/update")]
    public async Task<IActionResult> UpdateIndividualAsync(int id, IndividualUpdateDto individual,
        CancellationToken cancellationToken)
    {
        try
        {
            await _individualService.UpdateIndividualAsync(id, individual, cancellationToken);
            return NoContent();
        }
        catch (IndividualNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpDelete("individuals/{id}")]
    public async Task<IActionResult> DeleteIndividualAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _individualService.DeleteIndividualAsync(id, cancellationToken);
            return NoContent();
        }
        catch (IndividualNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost("individuals/create")]
    public async Task<IActionResult> CreateIndividualAsync(IndividualDto dto, CancellationToken cancellationToken)
    {
        await _individualService.CreateIndividualAsync(dto, cancellationToken);
        return Created();
    }
}