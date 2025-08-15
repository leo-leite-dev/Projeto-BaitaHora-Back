namespace BaitaHora.Application.IServices
{
    public interface ICompanyPositionQueries
    {
        Task<string[]> ListActiveNamesAsync(Guid companyId, CancellationToken ct = default);
        Task<bool> ExistsAsync(Guid companyId, string roleName, CancellationToken ct = default);
    }
}