using BaitaHora.Application.DTOs.Commands.Companies;
using BaitaHora.Domain.Entities.Companies;

namespace BaitaHora.Application.IServices.Companies
{
    public interface ICompanyService
    {
        Task<Company> CreateCompanyAsync(CreateCompanyCommand cmd, CancellationToken ct = default);
        Task UpdateCompanyAsync(UpdateCompanyCommand cmd, CancellationToken ct = default);
    }
}