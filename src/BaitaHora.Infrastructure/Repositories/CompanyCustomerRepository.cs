using BaitaHora.Application.IRepositories;
using BaitaHora.Domain.Entities.Companies.Customers;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories
{
    public sealed class CompanyCustomerRepository : ICompanyCustomerRepository
    {
        private readonly AppDbContext _context;
        public CompanyCustomerRepository(AppDbContext context) => _context = context;

        public Task AddAsync(CompanyCustomer e, CancellationToken ct = default)
            => _context.Set<CompanyCustomer>().AddAsync(e, ct).AsTask();

        public void Update(CompanyCustomer e, CancellationToken ct = default)
            => _context.Set<CompanyCustomer>().Update(e);

        public Task<CompanyCustomer?> GetAsync(Guid companyId, Guid customerId, CancellationToken ct = default)
            => _context.Set<CompanyCustomer>().AsNoTracking()
                .FirstOrDefaultAsync(x => x.CompanyId == companyId && x.CustomerId == customerId, ct);

        public Task<bool> ExistsAsync(Guid companyId, Guid customerId, CancellationToken ct = default)
            => _context.Set<CompanyCustomer>().AsNoTracking()
                .AnyAsync(x => x.CompanyId == companyId && x.CustomerId == customerId, ct);

        public Task<CompanyCustomer?> GetByCompanyAndPhoneAsync(
            Guid companyId, string phoneE164, CancellationToken ct = default)
        {
            var norm = phoneE164.Trim();

            return _context.Set<CompanyCustomer>()
                .AsNoTracking()
                .Where(cc => cc.CompanyId == companyId
                          && cc.Customer.PhoneE164 == norm)
                .FirstOrDefaultAsync(ct);
        }
    }
}