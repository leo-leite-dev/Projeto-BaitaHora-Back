using BaitaHora.Domain.Entities.Companies;
using BaitaHora.Domain.Enums;

namespace BaitaHora.Application.IRepositories
{
    public interface ICompanyMemberRepository : IGenericRepository<CompanyMember>
    {
        Task<CompanyMember?> GetAsync(Guid companyId, Guid userId, CancellationToken ct = default);
        Task<CompanyMember?> GetWithPositionAsync(Guid companyId, Guid userId, CancellationToken ct = default);
        Task<bool> IsActiveAsync(Guid companyId, Guid userId, CancellationToken ct = default);
        Task<CompanyMember?> FindAnyActiveAsync(Guid companyId, CancellationToken ct = default);
        Task<CompanyMember?> FindAnyActiveByPositionNameAsync(Guid companyId, string positionName, CancellationToken ct = default);
        Task<List<string>> ListActiveRolesAsync(Guid companyId, CancellationToken ct = default);
        Task<bool> AnyByRoleAsync(Guid companyId, CompanyRole role, bool onlyActive = true, CancellationToken ct = default);

    }
}