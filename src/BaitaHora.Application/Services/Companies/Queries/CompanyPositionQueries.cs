using BaitaHora.Application.IServices.Companies;

namespace BaitaHora.Application.Services.Companies.Queries
{
    public sealed class CompanyPositionQueries : ICompanyPositionQueries
    {
        private readonly ICompanyPositionRepository _companyPositionRepository;
        public CompanyPositionQueries(ICompanyPositionRepository companyPositionRepository)
        {
            _companyPositionRepository = companyPositionRepository ?? throw new ArgumentNullException(nameof(companyPositionRepository));
        }

        public Task<string[]> ListActiveNamesAsync(Guid companyId, CancellationToken ct = default)
            => _companyPositionRepository.ListActiveNamesAsync(companyId, ct);

        public async Task<bool> ExistsAsync(Guid companyId, string roleName, CancellationToken ct = default)
            => (await _companyPositionRepository.GetByNameAsync(companyId, roleName, ct)) is not null;
    }
}