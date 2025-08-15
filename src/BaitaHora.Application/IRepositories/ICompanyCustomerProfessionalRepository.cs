
using BaitaHora.Domain.Entities;

namespace BaitaHora.Application.IRepositories
{
    public interface ICompanyCustomerProfessionalRepository : IGenericRepository<CompanyCustomerProfessional>
    {
        Task<CompanyCustomerProfessional?> GetPrimaryAsync(Guid companyId, Guid customerId, CancellationToken ct = default);
        Task<IReadOnlyList<CompanyCustomerProfessional>> ListByCustomerAsync(Guid companyId, Guid customerId, bool onlyActive = true, CancellationToken ct = default);
        Task<bool> ExistsAsync(Guid companyId, Guid customerId, Guid professionalUserId, CancellationToken ct = default);
    }
}