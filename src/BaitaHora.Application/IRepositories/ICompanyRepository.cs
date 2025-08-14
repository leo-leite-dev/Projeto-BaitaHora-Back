using BaitaHora.Domain.Entities;

namespace BaitaHora.Application.IRepositories
{
    public interface ICompanyRepository : IGenericRepository<Company>
    {
        Task<bool> ExistsByNameAsync(string name);
        Task<bool> ExistsByDocumentAsync(string document);
        Task AddImageAsync(CompanyImage image);
    }
}