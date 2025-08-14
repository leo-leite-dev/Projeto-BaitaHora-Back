using BaitaHora.Application.IRepositories;
using BaitaHora.Application.IServices.Auths;
using BaitaHora.Domain.Enums;

namespace BaitaHora.Application.Services.Auths
{
    public class AccessService : IAccessService
    {
        private readonly ICompanyMemberRepository _memberRepo;

        public AccessService(ICompanyMemberRepository memberRepo)
            => _memberRepo = memberRepo;

        public async Task<bool> CanCreateUsersAsync(Guid actorUserId, Guid companyId)
        {
            var member = await _memberRepo.GetAsync(companyId, actorUserId);
            if (member is null) return false;

            return member.Role is CompanyRole.Owner or CompanyRole.Manager;
        }
    }
}