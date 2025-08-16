using BaitaHora.Domain.Enums;

public interface ICompanyMemberService
{
    Task AddMemberAsync(Guid companyId, Guid requesterUserId, Guid userId, CompanyRole role, bool isActive = true, CancellationToken ct = default);
    Task SetMemberPositionAsync(Guid companyId, Guid requesterUserId, Guid memberUserId, Guid positionId, CancellationToken ct = default);
}