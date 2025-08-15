using BaitaHora.Application.DTOs.Requests.Company;
using BaitaHora.Domain.Entities;
using BaitaHora.Domain.Enums;

namespace BaitaHora.Application.IServices
{
    public interface ICompanyService
    {
        Task<Company> CreateCompanyAsync(CompanyRequest request, CancellationToken ct = default);
        Task AddMemberAsync(Guid companyId, Guid userId, CompanyRole role, bool isActive = true, CancellationToken ct = default);
        Task UpdateCompanyAsync(Guid companyId, Guid requesterUserId, CompanyRequest companyRequest, CancellationToken ct = default);
        Task SetMemberPrimaryPositionAsync(Guid companyId, Guid requesterUserId, Guid memberUserId, Guid positionId, CancellationToken ct = default);
    }
}