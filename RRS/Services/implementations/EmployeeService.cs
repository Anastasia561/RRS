using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RRS.Data;
using RRS.Dtos;
using RRS.Exceptions;
using RRS.Models;

namespace RRS.Services;

public class EmployeeService : IEmployeeService
{
    private readonly DatabaseContext _context;

    public EmployeeService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<Employee> GetEmployeeAsync(LoginDto dto, CancellationToken cancellationToken)
    {
        var employee = await _context.Employees
            .Include(e => e.Role)
            .FirstOrDefaultAsync(e => e.Login == dto.Login, cancellationToken);

        if (employee == null)
            throw new InvalidCredentialsException();

        var hasher = new PasswordHasher<Employee>();
        var result = hasher.VerifyHashedPassword(employee, employee.Password, dto.Password);

        if (result == PasswordVerificationResult.Failed)
            throw new InvalidCredentialsException();

        return employee;
    }

    public async Task RegisterEmployeeAsync(RegisterDto dto, CancellationToken cancellationToken)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == dto.Role, cancellationToken);
        if (role == null)
            throw new RoleNotFoundException(dto.Role);

        var passwordHasher = new PasswordHasher<Employee>();
        var employee = new Employee
        {
            Login = dto.Login,
            RoleId = role.Id
        };

        employee.Password = passwordHasher.HashPassword(employee, dto.Password);

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync(cancellationToken);
    }
}