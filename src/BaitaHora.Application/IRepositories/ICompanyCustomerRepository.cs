
using BaitaHora.Domain.Entities.Customers;

namespace BaitaHora.Application.IRepositories
{
    public interface ICompanyCustomerRepository : IGenericRepository<CompanyCustomer>
    {
        Task<CompanyCustomer?> GetAsync(Guid companyId, Guid customerId, CancellationToken ct = default);
        Task<bool> ExistsAsync(Guid companyId, Guid customerId, CancellationToken ct = default);

        Task<CompanyCustomer?> GetByCompanyAndPhoneAsync(Guid companyId, string phoneE164, CancellationToken ct = default);
    }
}