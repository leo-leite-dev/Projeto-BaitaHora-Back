
using BaitaHora.Domain.Entities.Companies.Customers;

namespace BaitaHora.Application.IRepositories
{
    public interface ICompanyCustomerRepository
    {
        Task AddAsync(CompanyCustomer entity, CancellationToken ct = default);
        void Update(CompanyCustomer entity, CancellationToken ct = default);

        Task<CompanyCustomer?> GetAsync(Guid companyId, Guid customerId, CancellationToken ct = default);
        Task<bool> ExistsAsync(Guid companyId, Guid customerId, CancellationToken ct = default);
        Task<CompanyCustomer?> GetByCompanyAndPhoneAsync(Guid companyId, string phoneE164, CancellationToken ct = default);
    }
}