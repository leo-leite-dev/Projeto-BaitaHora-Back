using BaitaHora.Domain.Entities.Scheduling;

namespace BaitaHora.Application.IRepositories
{
    public interface IServiceCatalogItemRepository : IGenericRepository<ServiceCatalogItem>
    {
        Task<IReadOnlyList<ServiceCatalogItem>> GetByCompanyAsync(Guid companyId, bool onlyActive = true, CancellationToken ct = default);
        Task<ServiceCatalogItem?> GetByNameAsync(Guid companyId, string name, CancellationToken ct = default);
        Task<bool> ExistsNameAsync(Guid companyId, string name, CancellationToken ct = default);
    }
}