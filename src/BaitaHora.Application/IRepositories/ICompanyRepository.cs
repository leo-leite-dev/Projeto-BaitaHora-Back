using BaitaHora.Domain.Entities.Companies;

namespace BaitaHora.Application.IRepositories
{
    public interface ICompanyRepository : IGenericRepository<Company>
    {
        Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
        Task<bool> ExistsByDocumentAsync(string document, CancellationToken ct = default);
        Task AddImageAsync(CompanyImage image, CancellationToken ct = default);
    }
}