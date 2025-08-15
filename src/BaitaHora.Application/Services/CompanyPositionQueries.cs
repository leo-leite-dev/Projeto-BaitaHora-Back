using BaitaHora.Application.IRepositories;
using BaitaHora.Application.IServices;

namespace BaitaHora.Application.Services
{
    public sealed class CompanyPositionQueries : ICompanyPositionQueries
    {
        private readonly ICompanyPositionRepository _repo;
        public CompanyPositionQueries(ICompanyPositionRepository repo)
        {
            _repo = repo;
        }

        public Task<string[]> ListActiveNamesAsync(Guid companyId, CancellationToken ct = default)
            => _repo.ListActiveNamesAsync(companyId, ct);

        public async Task<bool> ExistsAsync(Guid companyId, string roleName, CancellationToken ct = default)
            => (await _repo.GetByNameAsync(companyId, roleName, ct)) is not null;
    }
}