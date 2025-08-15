using BaitaHora.Domain.Enums;

namespace BaitaHora.Application.IServices
{
    public interface ICompanyPermissionService
    {
        Task<bool> CanAsync(Guid companyId, Guid userId, CompanyRole required, CancellationToken ct = default);
        Task<CompanyRole?> GetEffectiveRoleAsync(Guid companyId, Guid userId, CancellationToken ct = default);
    }
}