using Microsoft.AspNetCore.Mvc;
using RRS.Dtos;
using RRS.Exceptions;
using RRS.Services;

namespace RRS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContractsController : ControllerBase
{
    private readonly IContractService _contractService;

    public ContractsController(IContractService contractService)
    {
        _contractService = contractService;
    }

    [HttpGet("{clientId}/all")]
    public async Task<IActionResult> GetAllForClientAsync(int clientId, CancellationToken cancellationToken)
    {
        try
        {
            var contracts = await _contractService.GetAllForClientAsync(clientId, cancellationToken);
            return Ok(contracts);
        }
        catch (ClientNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost("{clientId}/create")]
    public async Task<IActionResult> CreateContractAsync(int clientId, ContractCreateDto dto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _contractService.CreateContractAsync(clientId, dto, cancellationToken);
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("{contractId}/pay")]
    public async Task<IActionResult> PayForContractAsync(PaymentDto dto, int contractId,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            await _contractService.PayForContractAsync(dto, contractId, cancellationToken);
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("/pastDue")]
    public async Task<IActionResult> DeletePastDueContractsAsync(CancellationToken cancellationToken)
    {
        await _contractService.DeletePastDueContractsAsync(cancellationToken);
        return NoContent();
    }

    [HttpPut("/sign")]
    public async Task<IActionResult> MarkContractsAsSignedAsync(CancellationToken cancellationToken)
    {
        await _contractService.MarkContractsAsSignedAsync(cancellationToken);
        return NoContent();
    }
}