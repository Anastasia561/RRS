using Microsoft.AspNetCore.Mvc;
using RRS.Dtos;
using RRS.Exceptions;
using RRS.Services;

namespace RRS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwtTokenService;
    private readonly IEmployeeService _employeeService;

    public AuthController(IJwtService jwtTokenService, IEmployeeService employeeService)
    {
        _jwtTokenService = jwtTokenService;
        _employeeService = employeeService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _employeeService.GetEmployeeAsync(dto, cancellationToken);
            var token = _jwtTokenService.GenerateToken(user.Id.ToString(), user.Login, user.Role.Name);
            return Ok(new { Token = token });
        }
        catch (InvalidCredentialsException e)
        {
            return Unauthorized(e.Message);
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken cancellationToken)
    {
        try
        {
            await _employeeService.RegisterEmployeeAsync(dto, cancellationToken);
            return Ok("Registration succeed");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}