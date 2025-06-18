using RRS.Dtos;
using RRS.Models;

namespace RRS.Services;

public interface IEmployeeService
{
    public Task<Employee> GetEmployeeAsync(LoginDto dto, CancellationToken cancellationToken);
    public Task RegisterEmployeeAsync(RegisterDto dto, CancellationToken cancellationToken);
}