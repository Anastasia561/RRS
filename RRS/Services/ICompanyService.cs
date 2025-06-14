using RRS.Dtos;

namespace RRS.Services;

public interface ICompanyService
{
    public Task<List<CompanyDto>> GetAllCompaniesAsync(CancellationToken cancellationToken);
    public Task<CompanyDto> GetCompanyByIdAsync(int id, CancellationToken cancellationToken);
    public Task UpdateCompanyAsync(int id, CompanyUpdateDto dto, CancellationToken cancellationToken);
    public Task CreateCompanyAsync(CompanyCreateDto dto, CancellationToken cancellationToken);
}