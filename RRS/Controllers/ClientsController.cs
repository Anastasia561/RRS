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
    private readonly ICompanyService _companyService;

    public ClientsController(IIndividualService individualService, ICompanyService companyService)
    {
        _individualService = individualService;
        _companyService = companyService;
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
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

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
    public async Task<IActionResult> CreateIndividualAsync(IndividualCreateDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _individualService.CreateIndividualAsync(dto, cancellationToken);
        return Created();
    }

    [HttpGet("companies/all")]
    public async Task<IActionResult> GetAllCompaniesAsync(CancellationToken cancellationToken)
    {
        var result = await _companyService.GetAllCompaniesAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("companies/{id}")]
    public async Task<IActionResult> GetCompanyByIdAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _companyService.GetCompanyByIdAsync(id, cancellationToken));
        }
        catch (CompanyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost("companies/{id}/update")]
    public async Task<IActionResult> UpdateCompanyAsync(int id, CompanyUpdateDto dto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _companyService.UpdateCompanyAsync(id, dto, cancellationToken);
            return NoContent();
        }
        catch (CompanyNotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost("companies/create")]
    public async Task<IActionResult> CreateCompanyAsync(CompanyCreateDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _companyService.CreateCompanyAsync(dto, cancellationToken);
        return Created();
    }
}