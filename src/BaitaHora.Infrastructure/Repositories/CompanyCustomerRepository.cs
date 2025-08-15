using BaitaHora.Application.IRepositories;
using BaitaHora.Domain.Entities;
using BaitaHora.Domain.Entities.Customers;
using BaitaHora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BaitaHora.Infrastructure.Repositories
{
    public sealed class CompanyCustomerRepository : GenericRepository<CompanyCustomer>, ICompanyCustomerRepository
    {
        private readonly AppDbContext _ctx;
        public CompanyCustomerRepository(AppDbContext ctx) : base(ctx) { _ctx = ctx; }

        public Task<CompanyCustomer?> GetAsync(Guid companyId, Guid customerId, CancellationToken ct = default)
            => _ctx.Set<CompanyCustomer>().AsNoTracking()
                .FirstOrDefaultAsync(x => x.CompanyId == companyId && x.CustomerId == customerId, ct);

        public Task<bool> ExistsAsync(Guid companyId, Guid customerId, CancellationToken ct = default)
            => _ctx.Set<CompanyCustomer>().AsNoTracking()
                .AnyAsync(x => x.CompanyId == companyId && x.CustomerId == customerId, ct);

        public Task<CompanyCustomer?> GetByCompanyAndPhoneAsync(Guid companyId, string phoneE164, CancellationToken ct = default)
            => _ctx.Set<CompanyCustomer>().AsNoTracking()
                .Join(_ctx.Set<User>(),
                      cc => cc.CustomerId,
                      u => u.Id,
                      (cc, u) => new { cc, u })
                .Join(_ctx.Set<UserProfile>(),
                      x => x.u.ProfileId,
                      up => up.Id,
                      (x, up) => new { x.cc, up })
                .Where(x => x.cc.CompanyId == companyId && x.up.Phone == phoneE164)
                .Select(x => x.cc)
                .FirstOrDefaultAsync(ct);
    }
}