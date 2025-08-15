using BaitaHora.Domain.Enums;

namespace BaitaHora.Application.IServices
{
    public interface ICompanyPositionService
    {
        Task<Guid> CreateAsync(Guid companyId, Guid requesterUserId, string name, CompanyRole accessLevel, CancellationToken ct = default);
    }
}