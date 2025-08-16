using BaitaHora.Application.IRepositories;
using BaitaHora.Domain.Entities.Companies;

public interface ICompanyPositionRepository : IGenericRepository<CompanyPosition>
{
    Task<bool> ExistsByNameAsync(Guid companyId, string name, CancellationToken ct = default);
    Task<CompanyPosition?> GetByNameAsync(Guid companyId, string name, CancellationToken ct = default);
    Task<string[]> ListActiveNamesAsync(Guid companyId, CancellationToken ct = default);
    Task<IReadOnlyList<CompanyPosition>> ListActiveAsync(Guid companyId, CancellationToken ct = default);

    Task<CompanyPosition?> GetByIdWithinCompanyAsync(Guid companyId, Guid positionId, CancellationToken ct = default);
}
