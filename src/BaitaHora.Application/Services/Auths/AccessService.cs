using BaitaHora.Application.IRepositories;
using BaitaHora.Application.IServices.Auths;
using BaitaHora.Domain.Enums;

namespace BaitaHora.Application.Services.Auths
{
    public class AccessService : IAccessService
    {
        private readonly ICompanyMemberRepository _companyMemberRepository;

        public AccessService(ICompanyMemberRepository companyMemberRepository)
        {
            _companyMemberRepository = companyMemberRepository ?? throw new ArgumentNullException(nameof(companyMemberRepository));
        }

        public async Task<bool> CanCreateUsersAsync(Guid actorUserId, Guid companyId)
        {
            var member = await _companyMemberRepository.GetAsync(companyId, actorUserId);
            if (member is null) return false;

            return member.Role is CompanyRole.Owner or CompanyRole.Manager;
        }
    }
}