using BaitaHora.Application.IRepositories;
using BaitaHora.Application.IServices.Companies;
using BaitaHora.Domain.Enums;

namespace BaitaHora.Application.Services.Companies
{
    public sealed class CompanyPermissionService : ICompanyPermissionService
    {
        private readonly ICompanyMemberRepository _companyMemberRepository;

        public CompanyPermissionService(ICompanyMemberRepository companyMemberRepository)
        {
            _companyMemberRepository = companyMemberRepository ?? throw new ArgumentNullException(nameof(companyMemberRepository));
        }

        public async Task<bool> CanAsync(Guid companyId, Guid userId, CompanyRole required, CancellationToken ct = default)
        {
            var effective = await GetEffectiveRoleAsync(companyId, userId, ct);
            return effective.HasValue && HasAtLeast(effective.Value, required);
        }

        public async Task<CompanyRole?> GetEffectiveRoleAsync(Guid companyId, Guid userId, CancellationToken ct = default)
        {
            var member = await _companyMemberRepository.GetWithPositionAsync(companyId, userId, ct);
            if (member is null || !member.IsActive) return null;

            var effective = member.Role;

            var pos = member.PrimaryPosition;
            if (pos is not null && pos.IsActive)
            {
                if (pos.AccessLevel < effective)
                    effective = pos.AccessLevel;
            }

            return effective;
        }

        private static bool HasAtLeast(CompanyRole current, CompanyRole required)
        {
            return current <= required;
        }
    }
}