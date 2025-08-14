using BaitaHora.Domain.Entities;

namespace BaitaHora.Application.IRepositories
{
    public interface ICompanyMemberRepository
    {
        Task<CompanyMember?> GetAsync(Guid companyId, Guid userId);
        Task AddAsync(CompanyMember member);
        Task<bool> IsActiveAsync(Guid companyId, Guid userId);
    }
}