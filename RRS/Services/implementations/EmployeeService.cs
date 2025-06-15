using Microsoft.EntityFrameworkCore;
using RRS.Data;
using RRS.Dtos;
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
        var result = await _context.Employees
            .Include(e => e.Role).FirstOrDefaultAsync(
                u => u.Login == dto.Login && u.Password == dto.Password,
                cancellationToken);
        if (result == null)
            throw new InvalidOperationException();
        return result;
    }
}